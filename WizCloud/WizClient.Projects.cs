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
    /// Retrieves all projects from Wiz asynchronously.
    /// </summary>
    /// <param name="pageSize">The number of projects to retrieve per page. Defaults to 20.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all projects.</returns>
    public async Task<List<WizProject>> GetProjectsAsync(int pageSize = 20) {
        var projects = new List<WizProject>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetProjectsPageAsync(pageSize, endCursor);
            projects.AddRange(result.Projects);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return projects;
    }

    /// <summary>
    /// Streams projects from Wiz asynchronously as an <see cref="IAsyncEnumerable{WizProject}"/>.
    /// </summary>
    /// <param name="pageSize">The number of projects to retrieve per page. Defaults to 20.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable sequence of projects.</returns>
    public async IAsyncEnumerable<WizProject> GetProjectsAsyncEnumerable(int pageSize = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            var result = await GetProjectsPageAsync(pageSize, endCursor).ConfigureAwait(false);

            foreach (var project in result.Projects) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return project;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of projects from the Wiz API.
    /// </summary>
    /// <param name="first">The number of projects to retrieve.</param>
    /// <param name="after">The cursor for pagination, if retrieving subsequent pages.</param>
    /// <returns>A tuple containing the projects, whether there's a next page, and the cursor for the next page.</returns>
    private async Task<(List<WizProject> Projects, bool HasNextPage, string? EndCursor)> GetProjectsPageAsync(int first, string? after = null) {
        const string query = GraphQlQueries.ProjectsQuery;

        var variables = new {
            first,
            after
        };

        var requestBody = new {
            query,
            variables
        };

        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var projects = new List<WizProject>();
        var nodes = jsonResponse["data"]?["projects"]?["nodes"]?.AsArray();

        if (nodes != null) {
            foreach (var node in nodes) {
                if (node != null) {
                    projects.Add(new WizProject {
                        Id = node["id"]?.GetValue<string>() ?? string.Empty,
                        Name = node["name"]?.GetValue<string>() ?? string.Empty,
                        Slug = node["slug"]?.GetValue<string>() ?? string.Empty,
                        IsFolder = node["isFolder"]?.GetValue<bool>() ?? false
                    });
                }
            }
        }

        var pageInfo = jsonResponse["data"]?["projects"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (projects, hasNextPage, endCursor);
    }
}
