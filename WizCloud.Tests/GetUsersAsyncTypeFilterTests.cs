using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class GetUsersAsyncTypeFilterTests {
    private sealed class InspectingHandler : HttpMessageHandler {
        private readonly HttpResponseMessage _response;
        public string? LastRequestBody { get; private set; }

        public InspectingHandler(string responseContent) {
            _response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(responseContent)
            };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            return _response;
        }
    }

    [TestMethod]
    public async Task GetUsersAsync_SendsTypeFilterAndReturnsUsers() {
        var responseJson = "{\"data\":{\"cloudResourcesV2\":{\"pageInfo\":{\"hasNextPage\":false},\"nodes\":[{\"id\":\"1\",\"name\":\"A\",\"type\":\"USER_ACCOUNT\"},{\"id\":\"2\",\"name\":\"B\",\"type\":\"SERVICE_ACCOUNT\"}]}}}";
        var handler = new InspectingHandler(responseJson);
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var original = (HttpClient)field!.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token");
            var users = await client.GetUsersAsync(10, new[] { WizUserType.USER_ACCOUNT, WizUserType.SERVICE_ACCOUNT });
            Assert.AreEqual(2, users.Count);
            CollectionAssert.AreEquivalent(
                new[] { WizUserType.USER_ACCOUNT, WizUserType.SERVICE_ACCOUNT },
                users.Select(u => u.Type).ToArray());
            var json = JsonNode.Parse(handler.LastRequestBody!)!;
            var types = json["variables"]?["filterBy"]?["type"]?["equalsAnyOf"]?.AsArray().Select(n => n!.GetValue<string>()).ToArray();
            CollectionAssert.AreEquivalent(new[] { "USER_ACCOUNT", "SERVICE_ACCOUNT" }, types);
        } finally {
            field.SetValue(null, original);
        }
    }
}
