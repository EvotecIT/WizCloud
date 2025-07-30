using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;


namespace WizCloud;
/// <summary>
/// Provides a client for interacting with the Wiz GraphQL API.
/// </summary>
public class WizClient : IDisposable {
    private static readonly HttpClient _httpClient = CreateClient();
    private readonly string _apiEndpoint;
    private readonly string _token;
    private bool _disposed;

    private static HttpClient CreateClient() {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WizClient"/> class with a token and region string.
    /// </summary>
    /// <param name="token">The Wiz service account token for authentication.</param>
    /// <param name="region">The Wiz region identifier (e.g., "eu17", "us1"). Defaults to "eu17".</param>
    /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
    public WizClient(string token, string region = "eu17") {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        if (region is null)
            throw new ArgumentNullException(nameof(region));

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be empty or whitespace", nameof(region));

        _token = token;
        _apiEndpoint = $"https://api.{region}.app.wiz.io/graphql";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WizClient"/> class with a token and region enum.
    /// </summary>
    /// <param name="token">The Wiz service account token for authentication.</param>
    /// <param name="region">The Wiz region enumeration value.</param>
    /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
    public WizClient(string token, WizRegion? region) {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        if (region is null)
            throw new ArgumentNullException(nameof(region));

        var regionString = WizRegionHelper.ToApiString(region.Value);
        _token = token;
        _apiEndpoint = $"https://api.{regionString}.app.wiz.io/graphql";
    }

    /// <summary>
    /// Retrieves all users from Wiz asynchronously.
    /// </summary>
    /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all users.</returns>
    public async Task<List<WizUser>> GetUsersAsync(int pageSize = 20) {
        var users = new List<WizUser>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetUsersPageAsync(pageSize, endCursor);
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
    public async IAsyncEnumerable<WizUser> GetUsersAsyncEnumerable(int pageSize = 20, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            var result = await GetUsersPageAsync(pageSize, endCursor).ConfigureAwait(false);

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
    private async Task<(List<WizUser> Users, bool HasNextPage, string? EndCursor)> GetUsersPageAsync(int first, string? after = null) {
        var query = GraphQlQueries.UsersQuery;

        var variables = new {
            first,
            after,
            filterBy = new {
                type = new {
                    equals = new[] { "USER_ACCOUNT", "SERVICE_ACCOUNT", "GROUP", "ACCESS_KEY" }
                },
                property = new object[] { }
            }
        };

        var requestBody = new {
            query,
            variables
        };

        using (var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)) {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false)) {
                if (!response.IsSuccessStatusCode) {
                    var errorBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var message = $"Request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
                    if (!string.IsNullOrWhiteSpace(errorBody))
                        message += $" Body: {errorBody}";
                    throw new HttpRequestException(message);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jsonResponse = JsonNode.Parse(content);

                if (jsonResponse == null)
                    throw new InvalidOperationException("Received null response from API");

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

                return (users, hasNextPage, endCursor);
            }
        }
    }

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

        using (var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)) {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false)) {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jsonResponse = JsonNode.Parse(content);

                if (jsonResponse == null)
                    throw new InvalidOperationException("Received null response from API");

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
    }

    /// <summary>
    /// Releases all resources used by the WizClient.
    /// </summary>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the WizClient and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        _disposed = true;
    }
}