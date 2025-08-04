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
    public async Task<List<WizNetworkExposure>> GetNetworkExposuresAsync(
        int pageSize = 20,
        IEnumerable<int>? ports = null,
        IEnumerable<string>? protocols = null,
        bool? internetFacing = null,
        string? projectId = null) {
        var exposures = new List<WizNetworkExposure>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetNetworkExposuresPageAsync(pageSize, endCursor, ports, protocols, internetFacing, projectId).ConfigureAwait(false);
            exposures.AddRange(result.Exposures);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return exposures;
    }

    public async IAsyncEnumerable<WizNetworkExposure> GetNetworkExposuresAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<int>? ports = null,
        IEnumerable<string>? protocols = null,
        bool? internetFacing = null,
        string? projectId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizNetworkExposure> Exposures, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetNetworkExposuresPageAsync(pageSize, endCursor, ports, protocols, internetFacing, projectId).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var exposure in result.Exposures) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return exposure;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    private async Task<(List<WizNetworkExposure> Exposures, bool HasNextPage, string? EndCursor)> GetNetworkExposuresPageAsync(
        int first,
        string? after = null,
        IEnumerable<int>? ports = null,
        IEnumerable<string>? protocols = null,
        bool? internetFacing = null,
        string? projectId = null) {
        const string query = GraphQlQueries.NetworkExposureQuery;

        var portFilter = ports != null && ports.Any() ? new { equals = ports } : null;
        var protocolFilter = protocols != null && protocols.Any() ? new { equals = protocols } : null;
        var internetFacingFilter = internetFacing.HasValue ? new { equals = internetFacing.Value } : null;
        var projectFilter = projectId != null ? new { equals = new[] { projectId } } : null;

        var variables = new {
            first,
            after,
            filterBy = new {
                port = portFilter,
                protocol = protocolFilter,
                internetFacing = internetFacingFilter,
                projectId = projectFilter
            }
        };

        var requestBody = new { query, variables };

        using (var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint)) {
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

                var exposures = new List<WizNetworkExposure>();
                var nodes = jsonResponse["data"]?["networkExposure"]?["nodes"]?.AsArray();
                if (nodes != null) {
                    foreach (var node in nodes) {
                        if (node != null) {
                            var exposure = node.Deserialize<WizNetworkExposure>(s_jsonOptions);
                            if (exposure != null)
                                exposures.Add(exposure);
                        }
                    }
                }

                var pageInfo = jsonResponse["data"]?["networkExposure"]?["pageInfo"];
                bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
                string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

                return (exposures, hasNextPage, endCursor);
            }
        }
    }
}
