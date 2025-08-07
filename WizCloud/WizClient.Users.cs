using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WizCloud;

public partial class WizClient {
    /// <summary>
    /// Retrieves all users from Wiz asynchronously.
    /// </summary>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all users.</returns>
    public async Task<List<WizUser>> GetUsersAsync(
        int pageSize = 20,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null) {
        var users = new List<WizUser>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetUsersPageAsync(pageSize, endCursor, types, projectId);
            users.AddRange(result.Users);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return users;
    }

    /// <summary>
    /// Retrieves the total count of users from Wiz asynchronously.
    /// </summary>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
    /// <returns>The total number of users matching the criteria.</returns>
    public async Task<int> GetUsersCountAsync(
        IEnumerable<WizUserType>? types = null,
        string? projectId = null) {
        var query = GraphQlQueries.UsersCountQuery;
        var variables = new {
            filterBy = BuildUserFilters(types, projectId)
        };

        var requestBody = new {
            query,
            variables
        };
        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        return jsonResponse["data"]?["cloudResourcesV2"]?["totalCount"]?.GetValue<int>() ?? 0;
    }
    /// <summary>
    /// Streams users from Wiz asynchronously as an <see cref="IAsyncEnumerable{WizUser}"/>.
    /// Results are yielded in the same order they are retrieved.
    /// </summary>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
    /// <param name="degreeOfParallelism">Maximum number of pages to prefetch concurrently.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable sequence of users.</returns>
    public async IAsyncEnumerable<WizUser> GetUsersAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null,
        int degreeOfParallelism = 1,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {

        degreeOfParallelism = Math.Max(1, degreeOfParallelism);

        var cursorChannel = Channel.CreateUnbounded<(int Index, string? Cursor)>();
        var resultChannel = Channel.CreateUnbounded<(int Index, List<WizUser> Users)>();

        await cursorChannel.Writer.WriteAsync((0, null), cancellationToken).ConfigureAwait(false);
        var active = 1;

        async Task WorkerAsync() {
            while (await cursorChannel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false)) {
                while (cursorChannel.Reader.TryRead(out var item)) {
                    var (index, cursor) = item;
                    try {
                        var page = await GetUsersPageAsync(pageSize, cursor, types, projectId).ConfigureAwait(false);
                        await resultChannel.Writer.WriteAsync((index, page.Users), cancellationToken).ConfigureAwait(false);

                        if (page.HasNextPage) {
                            Interlocked.Increment(ref active);
                            await cursorChannel.Writer.WriteAsync((index + 1, page.EndCursor), cancellationToken).ConfigureAwait(false);
                        }
                    } catch (HttpRequestException) {
                        cursorChannel.Writer.Complete();
                        resultChannel.Writer.Complete();
                        return;
                    } finally {
                        if (Interlocked.Decrement(ref active) == 0)
                            cursorChannel.Writer.Complete();
                    }
                }
            }
        }

        var workers = Enumerable.Range(0, degreeOfParallelism)
            .Select(_ => Task.Run(WorkerAsync, cancellationToken))
            .ToArray();

        _ = Task.Run(async () => {
            try {
                await Task.WhenAll(workers).ConfigureAwait(false);
            } finally {
                resultChannel.Writer.Complete();
            }
        }, cancellationToken);

        var buffer = new SortedDictionary<int, List<WizUser>>();
        var nextIndex = 0;

        while (await resultChannel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false)) {
            while (resultChannel.Reader.TryRead(out var item)) {
                buffer[item.Index] = item.Users;

                while (buffer.TryGetValue(nextIndex, out var users)) {
                    foreach (var user in users) {
                        if (cancellationToken.IsCancellationRequested)
                            yield break;

                        yield return user;
                    }

                    buffer.Remove(nextIndex);
                    nextIndex++;
                }
            }
        }

        try {
            await resultChannel.Reader.Completion.ConfigureAwait(false);
        } catch (HttpRequestException) {
            yield break;
        }
    }


    /// <summary>
    /// Retrieves all users from Wiz with progress reporting.
    /// </summary>
    /// <param name="progressCallback">Optional callback to report progress.</param>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 500.</param>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
    /// <param name="degreeOfParallelism">Maximum number of pages to prefetch concurrently.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A list of all users matching the criteria.</returns>
    public async Task<List<WizUser>> GetAllUsersAsync(
        Action<int>? progressCallback = null,
        int pageSize = 500,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null,
        int degreeOfParallelism = 1,
        CancellationToken cancellationToken = default) {
        var users = new List<WizUser>();
        await foreach (var user in GetUsersAsyncEnumerable(pageSize, types, projectId, degreeOfParallelism, cancellationToken)) {
            if (cancellationToken.IsCancellationRequested)
                break;

            users.Add(user);
            progressCallback?.Invoke(users.Count);
        }

        return users;
    }

    /// <summary>
    /// Retrieves a single page of users from the Wiz API.
    /// </summary>
    /// <param name="first">The number of users to retrieve.</param>
    /// <param name="after">The cursor for pagination, if retrieving subsequent pages.</param>
    /// <returns>A tuple containing the users, whether there's a next page, and the cursor for the next page.</returns>
    private async Task<(List<WizUser> Users, bool HasNextPage, string? EndCursor)> GetUsersPageAsync(
        int first,
        string? after = null,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null) {
        var query = GraphQlQueries.UsersQuery;
        var variables = new {
            first,
            after,
            filterBy = BuildUserFilters(types, projectId)
        };

        var requestBody = new {
            query,
            variables
        };
        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var users = new List<WizUser>();
        var nodes = jsonResponse["data"]?["cloudResourcesV2"]?["nodes"]?.AsArray();

        if (nodes != null) {
            foreach (var node in nodes) {
                if (node != null)
                    users.Add(WizUser.FromJson(node));
            }
        }

        var pageInfo = jsonResponse["data"]?["cloudResourcesV2"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (users ?? new List<WizUser>(), hasNextPage, endCursor);
    }

    private static object? BuildUserFilters(IEnumerable<WizUserType>? types, string? projectId) {
        bool hasTypes = types != null && types.Any();
        bool hasProject = !string.IsNullOrEmpty(projectId);

        if (!hasTypes && !hasProject)
            return null;

        var filter = new Dictionary<string, object>();

        if (hasTypes) {
            var typeValues = types!.Select(t => t.ToString()).ToArray();
            filter["type"] = typeValues.Length == 1
                ? new { equals = typeValues[0] }
                : new { equalsAnyOf = typeValues };
        }

        if (hasProject) {
            var propertyFilter = new[] { new { name = "projectId", equals = new[] { projectId } } };
            filter["property"] = propertyFilter;
        }

        return filter;
    }
}
