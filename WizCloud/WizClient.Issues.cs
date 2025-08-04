using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud;

public partial class WizClient {
    /// <summary>
    /// Retrieves all issues from Wiz asynchronously.
    /// </summary>
    public async Task<List<WizIssue>> GetIssuesAsync(
        int pageSize = 20,
        IEnumerable<WizSeverity>? severities = null,
        IEnumerable<string>? statuses = null,
        string? projectId = null,
        IEnumerable<string>? types = null) {
        var issues = new List<WizIssue>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetIssuesPageAsync(pageSize, endCursor, severities, statuses, projectId, types).ConfigureAwait(false);
            issues.AddRange(result.Issues);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return issues;
    }

    /// <summary>
    /// Streams issues from Wiz asynchronously.
    /// </summary>
    public async IAsyncEnumerable<WizIssue> GetIssuesAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<WizSeverity>? severities = null,
        IEnumerable<string>? statuses = null,
        string? projectId = null,
        IEnumerable<string>? types = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizIssue> Issues, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetIssuesPageAsync(pageSize, endCursor, severities, statuses, projectId, types).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var issue in result.Issues) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return issue;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of issues from the Wiz API.
    /// </summary>
    private async Task<(List<WizIssue> Issues, bool HasNextPage, string? EndCursor)> GetIssuesPageAsync(
        int first,
        string? after = null,
        IEnumerable<WizSeverity>? severities = null,
        IEnumerable<string>? statuses = null,
        string? projectId = null,
        IEnumerable<string>? types = null) {
        const string query = GraphQlQueries.IssuesQuery;

        var severityFilter = severities != null && severities.Any()
            ? new { equals = severities.Select(s => s.ToString()) }
            : null;
        var statusFilter = statuses != null && statuses.Any()
            ? new { equals = statuses }
            : null;
        var typeFilter = types != null && types.Any()
            ? new { equals = types }
            : null;
        var projectFilter = projectId != null ? new { equals = new[] { projectId } } : null;

        var variables = new {
            first,
            after,
            filterBy = new {
                severity = severityFilter,
                status = statusFilter,
                type = typeFilter,
                projectId = projectFilter
            }
        };

        var requestBody = new { query, variables };

        using (var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)) {
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            using (var response = await SendWithRefreshAsync(request).ConfigureAwait(false)) {
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

                var issues = new List<WizIssue>();
                var nodes = jsonResponse["data"]?["issues"]?["nodes"]?.AsArray();

                if (nodes != null) {
                    foreach (var node in nodes) {
                        if (node != null)
                            issues.Add(WizIssue.FromJson(node));
                    }
                }

                var pageInfo = jsonResponse["data"]?["issues"]?["pageInfo"];
                bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
                string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

                return (issues, hasNextPage, endCursor);
            }
        }
    }
}
