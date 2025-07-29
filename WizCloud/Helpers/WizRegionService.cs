using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WizCloud;

/// <summary>
/// Provides methods to retrieve Wiz regions from the service.
/// </summary>
public static class WizRegionService {
    private static HttpClient _httpClient = CreateClient();
    private static List<WizRegion>? _cachedRegions;

    private static HttpClient CreateClient() {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    /// <summary>
    /// Gets the available Wiz regions from the service.
    /// </summary>
    /// <returns>A list of <see cref="WizRegion"/> values.</returns>
    public static async Task<IReadOnlyList<WizRegion>> GetAvailableRegionsAsync() {
        if (_cachedRegions is not null)
            return _cachedRegions;

        using var response = await _httpClient.GetAsync("https://auth.app.wiz.io/regions").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var json = JsonNode.Parse(content)?.AsArray() ?? throw new InvalidOperationException("Invalid regions response");

        var regions = new List<WizRegion>();
        foreach (var node in json) {
            if (node is not null) {
                var value = node.GetValue<string>();
                regions.Add(WizRegionHelper.FromString(value));
            }
        }

        _cachedRegions = regions;
        return regions;
    }
}
