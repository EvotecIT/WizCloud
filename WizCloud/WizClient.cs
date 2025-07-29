using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using WizCloud.Models;
using WizCloud.Enums;
using WizCloud.Helpers;

namespace WizCloud
{
    /// <summary>
    /// Provides a client for interacting with the Wiz GraphQL API.
    /// </summary>
    public class WizClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizClient"/> class with a token and region string.
        /// </summary>
        /// <param name="token">The Wiz service account token for authentication.</param>
        /// <param name="region">The Wiz region identifier (e.g., "eu17", "us1"). Defaults to "eu17".</param>
        /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
        public WizClient(string token, string region = "eu17")
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            if (region is null)
                throw new ArgumentNullException(nameof(region));

            _apiEndpoint = $"https://api.{region}.app.wiz.io/graphql";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizClient"/> class with a token and region enum.
        /// </summary>
        /// <param name="token">The Wiz service account token for authentication.</param>
        /// <param name="region">The Wiz region enumeration value.</param>
        /// <exception cref="ArgumentException">Thrown when the token is null or empty.</exception>
        public WizClient(string token, WizRegion? region)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            if (region is null)
                throw new ArgumentNullException(nameof(region));

            var regionString = WizRegionHelper.ToApiString(region.Value);
            _apiEndpoint = $"https://api.{regionString}.app.wiz.io/graphql";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Retrieves all users from Wiz asynchronously.
        /// </summary>
        /// <param name="pageSize">The number of users to retrieve per page. Defaults to 20.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all users.</returns>
        public async Task<List<WizUser>> GetUsersAsync(int pageSize = 20)
        {
            var users = new List<WizUser>();
            string? endCursor = null;
            bool hasNextPage = true;

            while (hasNextPage)
            {
                var result = await GetUsersPageAsync(pageSize, endCursor);
                users.AddRange(result.Users);
                hasNextPage = result.HasNextPage;
                endCursor = result.EndCursor;
            }

            return users;
        }

        /// <summary>
        /// Retrieves a single page of users from the Wiz API.
        /// </summary>
        /// <param name="first">The number of users to retrieve.</param>
        /// <param name="after">The cursor for pagination, if retrieving subsequent pages.</param>
        /// <returns>A tuple containing the users, whether there's a next page, and the cursor for the next page.</returns>
        private async Task<(List<WizUser> Users, bool HasNextPage, string? EndCursor)> GetUsersPageAsync(int first, string? after = null)
        {
            var query = @"query CloudIdentityPrincipals($first: Int, $after: String, $filterBy: CloudResourceV2Filters) { 
                cloudResourcesV2(first: $first, after: $after, filterBy: $filterBy) { 
                    pageInfo { hasNextPage endCursor } 
                    nodes { ...PrincipalDetails } 
                }
            } 
            fragment PrincipalDetails on CloudResourceV2 { 
                id name type nativeType deletedAt 
                graphEntity { id type properties } 
                hasAccessToSensitiveData hasAdminPrivileges hasHighPrivileges hasSensitiveData 
                projects { id name slug isFolder } 
                technology { id icon name categories { id name } description } 
                cloudAccount { id name cloudProvider externalId } 
                issueAnalytics { 
                    issueCount informationalSeverityCount lowSeverityCount 
                    mediumSeverityCount highSeverityCount criticalSeverityCount 
                }
            }";

            var variables = new
            {
                first,
                after,
                filterBy = new
                {
                    type = new
                    {
                        equals = new[] { "USER_ACCOUNT", "SERVICE_ACCOUNT", "GROUP", "ACCESS_KEY" }
                    },
                    property = new object[] { }
                }
            };

            var requestBody = new
            {
                query,
                variables
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, _apiEndpoint))
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(content);

                    if (jsonResponse == null)
                        throw new InvalidOperationException("Received null response from API");

                    var users = new List<WizUser>();
                    var nodes = jsonResponse["data"]?["cloudResourcesV2"]?["nodes"]?.AsArray();
                    
                    if (nodes != null)
                    {
                        foreach (var node in nodes)
                        {
                            if (node != null)
                                users.Add(WizUser.FromJson(node));
                        }
                    }

                    var pageInfo = jsonResponse["data"]?["cloudResourcesV2"]?["pageInfo"];
                    bool hasNextPage = pageInfo?["hasNextPage"]?.GetValue<bool>() ?? false;
                    string? endCursor = pageInfo?["endCursor"]?.GetValue<string>();

                    return (users, hasNextPage, endCursor);
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the WizClient.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the WizClient and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            
            _disposed = true;
        }
    }
}