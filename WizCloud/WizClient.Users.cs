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
    /// Streams users from Wiz asynchronously as an <see cref="IAsyncEnumerable{WizUser}"/>.
    /// </summary>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable sequence of users.</returns>
    public async IAsyncEnumerable<WizUser> GetUsersAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<WizUserType>? types = null,
        string? projectId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizUser> Users, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetUsersPageAsync(pageSize, endCursor, types, projectId).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var user in result.Users) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return user;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
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
                type = new { equals = typeFilter },
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
