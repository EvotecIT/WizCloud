using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
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

    private static void SetHttpClient(HttpClient client) {
        var field = typeof(WizRegionService).GetField("_httpClient", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field!.SetValue(null, client);
    }

    private static void ClearCache() {
        var field = typeof(WizRegionService).GetField("_cachedRegions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field!.SetValue(null, null);
    }}