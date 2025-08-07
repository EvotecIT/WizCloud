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
    /// Retrieves resources from Wiz.
    /// </summary>
    /// <param name="pageSize">The number of resources to retrieve per page.</param>
    /// <param name="types">Optional resource type filters.</param>
    /// <param name="cloudProviders">Optional cloud provider filters.</param>
    /// <param name="region">Optional region filter.</param>
    /// <param name="publiclyAccessible">Optional public accessibility filter.</param>
    /// <param name="tags">Optional tag filters.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <returns>A list of resources.</returns>
    public async Task<List<WizResource>> GetResourcesAsync(
        int pageSize = 20,
        IEnumerable<string>? types = null,
        IEnumerable<WizCloudProvider>? cloudProviders = null,
        string? region = null,
        bool? publiclyAccessible = null,
        IDictionary<string, string>? tags = null,
        string? projectId = null) {
        var resources = new List<WizResource>();
        string? endCursor = null;
        bool hasNextPage = true;

        while (hasNextPage) {
            var result = await GetResourcesPageAsync(
                pageSize,
                endCursor,
                types,
                cloudProviders,
                region,
                publiclyAccessible,
                tags,
                projectId).ConfigureAwait(false);
            resources.AddRange(result.Resources);
            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }

        return resources;
    }

    /// <summary>
    /// Streams resources from Wiz as an asynchronous sequence.
    /// </summary>
    /// <param name="pageSize">The number of resources to retrieve per page.</param>
    /// <param name="types">Optional resource type filters.</param>
    /// <param name="cloudProviders">Optional cloud provider filters.</param>
    /// <param name="region">Optional region filter.</param>
    /// <param name="publiclyAccessible">Optional public accessibility filter.</param>
    /// <param name="tags">Optional tag filters.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>An async enumerable of resources.</returns>
    public async IAsyncEnumerable<WizResource> GetResourcesAsyncEnumerable(
        int pageSize = 20,
        IEnumerable<string>? types = null,
        IEnumerable<WizCloudProvider>? cloudProviders = null,
        string? region = null,
        bool? publiclyAccessible = null,
        IDictionary<string, string>? tags = null,
        string? projectId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        string? endCursor = null;
        bool hasNextPage = true;

        while (!cancellationToken.IsCancellationRequested && hasNextPage) {
            (List<WizResource> Resources, bool HasNextPage, string? EndCursor) result;
            try {
                result = await GetResourcesPageAsync(
                    pageSize,
                    endCursor,
                    types,
                    cloudProviders,
                    region,
                    publiclyAccessible,
                    tags,
                    projectId).ConfigureAwait(false);
            } catch (HttpRequestException) {
                yield break;
            }

            foreach (var resource in result.Resources) {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return resource;
            }

            hasNextPage = result.HasNextPage;
            endCursor = result.EndCursor;
        }
    }

    /// <summary>
    /// Retrieves a single page of resources from the Wiz API.
    /// </summary>
    /// <param name="first">The number of resources to retrieve.</param>
    /// <param name="after">Cursor for pagination.</param>
    /// <param name="types">Optional resource type filters.</param>
    /// <param name="cloudProviders">Optional cloud provider filters.</param>
    /// <param name="region">Optional region filter.</param>
    /// <param name="publiclyAccessible">Optional public accessibility filter.</param>
    /// <param name="tags">Optional tag filters.</param>
    /// <param name="projectId">Optional project identifier filter.</param>
    /// <returns>A tuple containing the resources, pagination flag, and end cursor.</returns>
    private async Task<(List<WizResource> Resources, bool HasNextPage, string? EndCursor)> GetResourcesPageAsync(
        int first,
        string? after = null,
        IEnumerable<string>? types = null,
        IEnumerable<WizCloudProvider>? cloudProviders = null,
        string? region = null,
        bool? publiclyAccessible = null,
        IDictionary<string, string>? tags = null,
        string? projectId = null) {
        const string query = GraphQlQueries.ResourcesQuery;

        var typeFilter = types != null && types.Any() ? new { equals = types } : null;
        var cloudProviderFilter = cloudProviders != null && cloudProviders.Any()
            ? new { equals = cloudProviders.Select(cp => cp.ToString()) }
            : null;
        var regionFilter = region != null ? new { equals = new[] { region } } : null;
        var publicFilter = publiclyAccessible.HasValue ? new { equals = publiclyAccessible.Value } : null;
        var tagFilter = tags != null && tags.Count > 0
            ? tags.Select(kvp => new { name = kvp.Key, equals = new[] { kvp.Value } }).ToArray()
            : Array.Empty<object>();
        var projectFilter = projectId != null ? new { equals = new[] { projectId } } : null;

        var variables = new {
            first,
            after,
            filterBy = new {
                type = typeFilter,
                cloudPlatform = cloudProviderFilter,
                region = regionFilter,
                publiclyAccessible = publicFilter,
                tags = tagFilter,
                projectId = projectFilter
            }
        };

        var requestBody = new { query, variables };

        var jsonResponse = await SendGraphQlRequestAsync(requestBody).ConfigureAwait(false);

        var resources = new List<WizResource>();
        var nodes = jsonResponse["data"]?["resources"]?["nodes"]?.AsArray();

        if (nodes != null) {
            foreach (var node in nodes) {
                if (node != null)
                    resources.Add(WizResource.FromJson(node));
            }
        }

        var pageInfo = jsonResponse["data"]?["resources"]?["pageInfo"];
        bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
        string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

        return (resources, hasNextPage, endCursor);
    }
}
