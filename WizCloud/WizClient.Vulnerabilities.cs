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
    /// Retrieves all vulnerabilities from Wiz asynchronously.
    /// </summary>
    public async Task<List<WizVulnerability>> GetVulnerabilitiesAsync(
        int pageSize = 20,
        string? cve = null,
        double? minCvss = null,
        bool? exploitAvailable = null,
        string? projectId = null) {
        var vulnerabilities = new List<WizVulnerability>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetVulnerabilitiesPageAsync(
                pageSize,
                endCursor,
                cve,
                minCvss,
                exploitAvailable,
                projectId).ConfigureAwait(false);
            vulnerabilities.AddRange(result.Vulnerabilities);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return vulnerabilities;
    }

    /// <summary>
    /// Streams vulnerabilities from Wiz asynchronously.
    /// </summary>
    public async IAsyncEnumerable<WizVulnerability> GetVulnerabilitiesAsyncEnumerable(
        int pageSize = 20,
        string? cve = null,
        double? minCvss = null,
        bool? exploitAvailable = null,
        string? projectId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizVulnerability> Vulnerabilities, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetVulnerabilitiesPageAsync(
                    pageSize,
                    endCursor,
                    cve,
                    minCvss,
                    exploitAvailable,
                    projectId).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var vulnerability in result.Vulnerabilities) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return vulnerability;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of vulnerabilities from the Wiz API.
    /// </summary>
    private async Task<(List<WizVulnerability> Vulnerabilities, bool HasNextPage, string? EndCursor)> GetVulnerabilitiesPageAsync(
        int first,
        string? after = null,
        string? cve = null,
        double? minCvss = null,
        bool? exploitAvailable = null,
        string? projectId = null) {
        const string query = GraphQlQueries.VulnerabilitiesQuery;

        var cveFilter = cve != null ? new { equals = new[] { cve } } : null;
        var cvssFilter = minCvss != null ? new { score = new { gte = minCvss } } : null;
        var exploitFilter = exploitAvailable.HasValue ? new { equals = exploitAvailable.Value } : null;
        var projectFilter = projectId != null ? new { equals = new[] { projectId } } : null;

        var variables = new {
            first,
            after,
            filterBy = new {
                cve = cveFilter,
                cvss = cvssFilter,
                exploitAvailable = exploitFilter,
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

                var vulnerabilities = new List<WizVulnerability>();
                var nodes = jsonResponse["data"]?["vulnerabilities"]?["nodes"]?.AsArray();

                if (nodes != null) {
                    foreach (var node in nodes) {
                        if (node != null) {
                            var vul = node.Deserialize<WizVulnerability>(s_jsonOptions);
                            if (vul != null)
                                vulnerabilities.Add(vul);
                        }
                    }
                }

                var pageInfo = jsonResponse["data"]?["vulnerabilities"]?["pageInfo"];
                bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
                string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

                return (vulnerabilities, hasNextPage, endCursor);
            }
        }
    }
}
