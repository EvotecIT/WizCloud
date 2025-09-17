using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud;

public partial class WizClient {
    /// <summary>
    /// Retrieves all users from Wiz asynchronously.
    /// </summary>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
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
    /// Streams users from Wiz with progress and optional limits.
    /// </summary>
    /// <param name="pageSize">Number of users per page. Default is 500.</param>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
    /// <param name="maxResults">Optional maximum number of users to retrieve.</param>
    /// <param name="progress">Optional progress reporter receiving retrieved and total counts.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable sequence of users.</returns>
    public async IAsyncEnumerable<WizUser> GetUsersWithProgressAsyncEnumerable(
        int pageSize = 500,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null,
        int? maxResults = null,
        IProgress<WizProgress>? progress = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        int retrieved = 0;
        var total = await GetUsersCountAsync(types, projectId).ConfigureAwait(false);
        var effectiveMax = maxResults.HasValue ? Math.Min(maxResults.Value, total) : total;

        progress?.Report(new WizProgress(retrieved, effectiveMax));

        await foreach (var user in GetUsersAsyncEnumerable(pageSize, types, projectId, cancellationToken)) {
            if (cancellationToken.IsCancellationRequested) {
                yield break;
            }

            yield return user;
            retrieved++;

            progress?.Report(new WizProgress(retrieved, effectiveMax));

            if (retrieved >= effectiveMax) {
                yield break;
            }
        }
    }
    /// <summary>
    /// Streams users from Wiz asynchronously as an <see cref="IAsyncEnumerable{WizUser}"/>.
    /// Pages are requested one at a time while the next page is prefetched in the background.
    /// </summary>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable sequence of users.</returns>
    public async IAsyncEnumerable<WizUser> GetUsersAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {

        Task<(List<WizUser> Users, bool HasNextPage, string? EndCursor)>? nextTask;

        (List<WizUser> Users, bool HasNextPage, string? EndCursor) page;
        try {
            page = await GetUsersPageAsync(pageSize, null, types, projectId).ConfigureAwait(false);
        } catch (HttpRequestException) {
            yield break;
        }

        nextTask = page.HasNextPage
            ? GetUsersPageAsync(pageSize, page.EndCursor, types, projectId)
            : null;

        foreach (var user in page.Users) {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return user;
        }

        while (nextTask != null) {
            try {
                page = await nextTask.ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            nextTask = page.HasNextPage
                ? GetUsersPageAsync(pageSize, page.EndCursor, types, projectId)
                : null;

            foreach (var user in page.Users) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return user;
            }
        }
    }


    /// <summary>
    /// Retrieves all users from Wiz with progress reporting.
    /// </summary>
    /// <param name="progressCallback">Optional callback to report progress.</param>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 500.</param>
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A list of all users matching the criteria.</returns>
    public async Task<List<WizUser>> GetAllUsersAsync(
        Action<int>? progressCallback = null,
        int pageSize = 500,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null,
        CancellationToken cancellationToken = default) {
        var users = new List<WizUser>();
        await foreach (var user in GetUsersAsyncEnumerable(pageSize, types, projectId, cancellationToken)) {
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
    /// <param name="types">Optional filter for user types.</param>
    /// <param name="projectId">Optional project ID filter.</param>
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