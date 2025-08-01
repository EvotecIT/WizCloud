using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace WizCloud;
/// <summary>
/// Provides methods to acquire Wiz service account tokens using client credentials.
/// </summary>
public static class WizAuthentication {
    /// <summary>
    /// Obtains a service account token from Wiz using a client ID and client secret.
    /// </summary>
    /// <param name="clientId">The service account client ID.</param>
    /// <param name="clientSecret">The service account client secret.</param>
    /// <param name="region">The Wiz region parameter (currently not used for authentication endpoint).</param>
    /// <returns>The service account token.</returns>
    public static async Task<string> AcquireTokenAsync(string clientId, string clientSecret, WizRegion region = WizRegion.EU17) {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new ArgumentException("Client secret cannot be null or empty", nameof(clientSecret));

        string authEndpoint = "https://auth.app.wiz.io/oauth/token";

        using var httpClient = new HttpClient();
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "client_credentials" },
            { "audience", "wiz-api" }
        });

        using var response = await httpClient.PostAsync(authEndpoint, content).ConfigureAwait(false);
        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode) {
            var message = $"Authentication failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
            if (!string.IsNullOrWhiteSpace(responseString))
                message += $" Body: {responseString}";
            throw new HttpRequestException(message);
        }

        using var document = JsonDocument.Parse(responseString);
        if (!document.RootElement.TryGetProperty("access_token", out JsonElement tokenElement))
            throw new InvalidOperationException("Token was not found in the authentication response.");

        return tokenElement.GetString() ?? throw new InvalidOperationException("Token was null in the authentication response.");
    }
}