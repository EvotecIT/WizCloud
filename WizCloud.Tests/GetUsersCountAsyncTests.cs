using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class GetUsersCountAsyncTests {
    private sealed class FakeHandler : HttpMessageHandler {
        private readonly HttpResponseMessage _response;

        public FakeHandler(string content) {
            _response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return Task.FromResult(_response);
        }
    }

    [TestMethod]
    public async Task GetUsersCountAsync_ReturnsParsedCount() {
        var handler = new FakeHandler("{\"data\":{\"cloudResourcesV2\":{\"totalCount\":5}}}");
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var original = (HttpClient)field!.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token");
            var count = await client.GetUsersCountAsync();
            Assert.AreEqual(5, count);
        } finally {
            field.SetValue(null, original);
        }
    }
}
