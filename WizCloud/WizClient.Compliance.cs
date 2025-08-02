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
    public async Task<List<WizComplianceResult>> GetCompliancePostureAsync(
        IEnumerable<string>? frameworks = null,
        double? minScore = null) {
        const string query = GraphQlQueries.CompliancePostureQuery;

        var variables = new {
            frameworks = frameworks?.ToArray()
        };

        var requestBody = new { query, variables };

        using (var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)) {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

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

                var results = new List<WizComplianceResult>();
                var nodes = jsonResponse["data"]?["compliancePosture"]?.AsArray();
                if (nodes != null) {
                    foreach (var node in nodes) {
                        if (node != null) {
                            var result = WizComplianceResult.FromJson(node);
                            if (!minScore.HasValue || (result.OverallScore ?? 0) >= minScore.Value)
                                results.Add(result);
                        }
                    }
                }

                return results;
            }
        }
    }

    public async IAsyncEnumerable<WizComplianceResult> GetCompliancePostureAsyncEnumerable(
        IEnumerable<string>? frameworks = null,
        double? minScore = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        List<WizComplianceResult> results;
        try {
            results = await GetCompliancePostureAsync(frameworks, minScore).ConfigureAwait(false);
        } catch (HttpRequestException) {
            yield break;
        }

        foreach (var result in results) {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return result;
        }
    }
}
