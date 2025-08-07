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
/// <summary>
/// Provides a client for interacting with the Wiz GraphQL API.
/// </summary>
public partial class WizClient : IDisposable {
    private static HttpClient _httpClient = CreateClient();
    private readonly string _apiEndpoint;
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly WizRegion _region;
    private string _token;
    private bool _disposed;

    private static HttpClient CreateClient() {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WizClient"/> class.
    /// </summary>
    /// <param name="token">The Wiz service account token for authentication.</param>
    /// <param name="region">The Wiz region enumeration value. Defaults to <see cref="WizRegion.EU17"/>.</param>
    /// <param name="clientId">Optional Wiz service account client ID used for token refresh.</param>
    /// <param name="clientSecret">Optional Wiz service account client secret used for token refresh.</param>
    /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
    public WizClient(string token, WizRegion region = WizRegion.EU17, string? clientId = null, string? clientSecret = null) {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        _token = token;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _region = region;
        var regionString = WizRegionHelper.ToApiString(region);
        _apiEndpoint = $"https://api.{regionString}.app.wiz.io/graphql";
    }

    /// <summary>
    /// Creates a new instance of the <see cref="WizClient"/> class using client credentials.
    /// </summary>
    /// <param name="clientId">The Wiz service account client ID.</param>
    /// <param name="clientSecret">The Wiz service account client secret.</param>
    /// <param name="region">The Wiz region enumeration value.</param>
    /// <returns>A <see cref="WizClient"/> instance authenticated with the retrieved token.</returns>
    public static async Task<WizClient> CreateAsync(string clientId, string clientSecret, WizRegion region = WizRegion.EU17) {
        var token = await WizAuthentication.AcquireTokenAsync(clientId, clientSecret, region).ConfigureAwait(false);
        return new WizClient(token, region, clientId, clientSecret);
    }

    /// <inheritdoc cref="CreateAsync(string, string, WizRegion)"/>
    /// <param name="clientId">The Wiz service account client ID.</param>
    /// <param name="clientSecret">The Wiz service account client secret.</param>
    /// <param name="region">The Wiz region identifier.</param>
    public static Task<WizClient> CreateAsync(string clientId, string clientSecret, string region)
        => CreateAsync(clientId, clientSecret, WizRegionHelper.FromString(region));

    private async Task<HttpResponseMessage> SendWithRefreshAsync(HttpRequestMessage request) {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Unauthorized &&
            !string.IsNullOrEmpty(_clientId) &&
            !string.IsNullOrEmpty(_clientSecret)) {
            response.Dispose();
            _token = await WizAuthentication.AcquireTokenAsync(_clientId!, _clientSecret!, _region).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        }

        return response;
    }

    private async Task<JsonNode> SendGraphQlRequestAsync(object requestBody) {
        using var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint) {
            Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            )
        };

        using var response = await SendWithRefreshAsync(request).ConfigureAwait(false);
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

        return jsonResponse;
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