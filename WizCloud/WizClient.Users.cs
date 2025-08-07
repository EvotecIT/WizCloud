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

        var typeFilter = types != null && types.Any()
            ? types.Select(t => t.ToString())
            : new[] { "USER_ACCOUNT", "SERVICE_ACCOUNT", "GROUP", "ACCESS_KEY" };

        var propertyFilter = projectId != null
            ? new object[] { new { name = "projectId", equals = new[] { projectId } } }
            : Array.Empty<object>();

        var variables = new {
            filterBy = new {
                type = new { equalsAnyOf = typeFilter },
                property = propertyFilter
            }
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
        try {
            await Task.CompletedTask.ConfigureAwait(false);
        } catch (HttpRequestException) {
            yield break;
        }

        var channel = Channel.CreateBounded<(int Index, List<WizUser> Users)>(degreeOfParallelism);

        _ = Task.Run(async () => {
            try {
                string? endCursor = null;
                    bool hasNextPage = true;
                    var index = 0;

                    while (!cancellationToken.IsCancellationRequested && hasNextPage) {
                        var result = await GetUsersPageAsync(pageSize, endCursor, types, projectId).ConfigureAwait(false);
                        await channel.Writer.WriteAsync((index, result.Users), cancellationToken).ConfigureAwait(false);
                        hasNextPage = result.HasNextPage;
                        endCursor = result.EndCursor;
                        index++;
                    }
                } catch (HttpRequestException) {
                } finally {
                    channel.Writer.Complete();
                }
            }, cancellationToken);

            var buffer = new SortedDictionary<int, List<WizUser>>();
            var nextIndex = 0;

            while (await channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false)) {
                while (channel.Reader.TryRead(out var item)) {
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
                await channel.Reader.Completion.ConfigureAwait(false);
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

        var typeFilter = types != null && types.Any()
            ? types.Select(t => t.ToString())
            : new[] { "USER_ACCOUNT", "SERVICE_ACCOUNT", "GROUP", "ACCESS_KEY" };

        var propertyFilter = projectId != null
            ? new object[] { new { name = "projectId", equals = new[] { projectId } } }
            : Array.Empty<object>();

        var variables = new {
            first,
            after,
            filterBy = new {
                type = new { equalsAnyOf = typeFilter },
                property = propertyFilter
            }
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
}
