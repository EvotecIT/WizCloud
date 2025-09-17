using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud;

public partial class WizClient {
    /// <summary>
    /// Retrieves network exposures from Wiz.
    /// </summary>
    /// <param name="pageSize">The number of exposures to retrieve per page.</param>
    /// <param name="ports">Optional port filters.</param>
    /// <param name="protocols">Optional protocol filters.</param>
    /// <param name="internetFacing">Optional internet-facing filter.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <returns>A list of network exposures.</returns>
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

    /// <summary>
    /// Streams network exposures from Wiz as an asynchronous sequence.
    /// </summary>
    /// <param name="pageSize">The number of exposures to retrieve per page.</param>
    /// <param name="ports">Optional port filters.</param>
    /// <param name="protocols">Optional protocol filters.</param>
    /// <param name="internetFacing">Optional internet-facing filter.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable of network exposures.</returns>
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

    /// <summary>
    /// Retrieves a single page of network exposures from the Wiz API.
    /// </summary>
    /// <param name="first">The number of exposures to retrieve.</param>
    /// <param name="after">Cursor for pagination.</param>
    /// <param name="ports">Optional port filters.</param>
    /// <param name="protocols">Optional protocol filters.</param>
    /// <param name="internetFacing">Optional internet-facing filter.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <returns>A tuple containing the exposures, pagination flag, and end cursor.</returns>
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

        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var exposures = new List<WizNetworkExposure>();
        var nodes = jsonResponse["data"]?["networkExposure"]?["nodes"]?.AsArray();
        if (nodes != null) {
            foreach (var node in nodes) {
                if (node != null)
                    exposures.Add(WizNetworkExposure.FromJson(node));
            }
        }

        var pageInfo = jsonResponse["data"]?["networkExposure"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (exposures, hasNextPage, endCursor);
    }
}