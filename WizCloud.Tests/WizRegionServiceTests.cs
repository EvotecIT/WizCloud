using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class WizRegionServiceTests {
    private sealed class FakeHandler : HttpMessageHandler {
        private readonly HttpResponseMessage _response;
        public int CallCount { get; private set; }

        public FakeHandler(string content) {
            _response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            CallCount++;
            return Task.FromResult(_response);
        }
    }

    private sealed class ErrorHandler : HttpMessageHandler {
        private readonly HttpResponseMessage _response;

        public ErrorHandler(string content) {
            _response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(content) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(_response);
    }

    [TestMethod]
    public async Task GetAvailableRegionsAsync_ReturnsParsedValues() {
        var handler = new FakeHandler("[\"eu1\",\"us1\"]");
        SetHttpClient(new HttpClient(handler));
        ClearCache();

        var regions = await WizRegionService.GetAvailableRegionsAsync();

        Assert.AreEqual(2, regions.Count);
        Assert.AreEqual(WizRegion.EU1, regions[0]);
        Assert.AreEqual(WizRegion.US1, regions[1]);
    }

    [TestMethod]
    public async Task GetAvailableRegionsAsync_CachesResults() {
        var handler1 = new FakeHandler("[\"eu1\"]");
        SetHttpClient(new HttpClient(handler1));
        ClearCache();

        var first = await WizRegionService.GetAvailableRegionsAsync();

        var handler2 = new FakeHandler("[\"us1\"]");
        SetHttpClient(new HttpClient(handler2));

        var second = await WizRegionService.GetAvailableRegionsAsync();

        CollectionAssert.AreEqual(first.ToList(), second.ToList());
        Assert.AreEqual(0, handler2.CallCount);
    }

    [TestMethod]
    public async Task GetAvailableRegionsAsync_MultipleConcurrentCalls_OnlyRequestsOnce() {
        var handler = new FakeHandler("[\"eu1\"]");
        SetHttpClient(new HttpClient(handler));
        ClearCache();

        var tasks = Enumerable.Range(0, 5).Select(_ => WizRegionService.GetAvailableRegionsAsync());
        var results = await Task.WhenAll(tasks);

        foreach (var regions in results) {
            Assert.AreEqual(1, regions.Count);
            Assert.AreEqual(WizRegion.EU1, regions[0]);
        }

        Assert.AreEqual(1, handler.CallCount);
    }

    [TestMethod]
    public async Task GetAvailableRegionsAsync_FailureIncludesResponseBody() {
        var handler = new ErrorHandler("boom");
        SetHttpClient(new HttpClient(handler));
        ClearCache();

        var ex = await Assert.ThrowsExceptionAsync<HttpRequestException>(WizRegionService.GetAvailableRegionsAsync);
        StringAssert.Contains(ex.Message, "boom");
    }

    private static void SetHttpClient(HttpClient client) {
        var field = typeof(WizRegionService).GetField("_httpClient", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field!.SetValue(null, client);
    }

    private static void ClearCache() {
        var field = typeof(WizRegionService).GetField("_cachedRegions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var method = typeof(WizRegionService).GetMethod("CreateLazy", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(method);
        var lazy = method!.Invoke(null, null);
        field!.SetValue(null, lazy);
    }
}
