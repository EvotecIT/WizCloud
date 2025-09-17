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
    /// Retrieves configuration findings from Wiz.
    /// </summary>
    /// <param name="pageSize">The number of findings to retrieve per page.</param>
    /// <param name="frameworks">Optional compliance frameworks filter.</param>
    /// <param name="severities">Optional severity filter.</param>
    /// <param name="categories">Optional category filter.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <returns>A list of configuration findings.</returns>
    public async Task<List<WizConfigurationFinding>> GetConfigurationFindingsAsync(
        int pageSize = 20,
        IEnumerable<string>? frameworks = null,
        IEnumerable<WizSeverity>? severities = null,
        IEnumerable<string>? categories = null,
        string? projectId = null) {
        var findings = new List<WizConfigurationFinding>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetConfigurationFindingsPageAsync(pageSize, endCursor, frameworks, severities, categories, projectId).ConfigureAwait(false);
            findings.AddRange(result.Findings);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return findings;
    }

    /// <summary>
    /// Streams configuration findings from Wiz as an asynchronous sequence.
    /// </summary>
    /// <param name="pageSize">The number of findings to retrieve per page.</param>
    /// <param name="frameworks">Optional compliance frameworks filter.</param>
    /// <param name="severities">Optional severity filter.</param>
    /// <param name="categories">Optional category filter.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable of configuration findings.</returns>
    public async IAsyncEnumerable<WizConfigurationFinding> GetConfigurationFindingsAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<string>? frameworks = null,
        IEnumerable<WizSeverity>? severities = null,
        IEnumerable<string>? categories = null,
        string? projectId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizConfigurationFinding> Findings, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetConfigurationFindingsPageAsync(pageSize, endCursor, frameworks, severities, categories, projectId).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var finding in result.Findings) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return finding;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of configuration findings from the Wiz API.
    /// </summary>
    /// <param name="first">The number of findings to retrieve.</param>
    /// <param name="after">Cursor for pagination.</param>
    /// <param name="frameworks">Optional compliance frameworks filter.</param>
    /// <param name="severities">Optional severity filter.</param>
    /// <param name="categories">Optional category filter.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <returns>A tuple containing the findings, pagination flag, and end cursor.</returns>
    private async Task<(List<WizConfigurationFinding> Findings, bool HasNextPage, string? EndCursor)> GetConfigurationFindingsPageAsync(
        int first,
        string? after = null,
        IEnumerable<string>? frameworks = null,
        IEnumerable<WizSeverity>? severities = null,
        IEnumerable<string>? categories = null,
        string? projectId = null) {
        const string query = GraphQlQueries.ConfigurationFindingsQuery;

        var frameworkFilter = frameworks != null && frameworks.Any() ? new { equals = frameworks } : null;
        var severityFilter = severities != null && severities.Any() ? new { equals = severities.Select(s => s.ToString()) } : null;
        var categoryFilter = categories != null && categories.Any() ? new { equals = categories } : null;
        var projectFilter = projectId != null ? new { equals = new[] { projectId } } : null;

        var variables = new {
            first,
            after,
            filterBy = new {
                framework = frameworkFilter,
                severity = severityFilter,
                category = categoryFilter,
                projectId = projectFilter
            }
        };
        var requestBody = new { query, variables };

        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var findings = new List<WizConfigurationFinding>();
        var nodes = jsonResponse["data"]?["configurationFindings"]?["nodes"]?.AsArray();
        if (nodes != null) {
            foreach (var node in nodes) {
                if (node != null)
                    findings.Add(WizConfigurationFinding.FromJson(node));
            }
        }

        var pageInfo = jsonResponse["data"]?["configurationFindings"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (findings, hasNextPage, endCursor);
    }
}