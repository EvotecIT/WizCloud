using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class GetUsersParallelTests {
    private sealed class PagingHandler : HttpMessageHandler {
        private int _callCount;
        public int CallCount => _callCount;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            _callCount++;
            string content = _callCount switch {
                1 => "{\"data\":{\"cloudResourcesV2\":{\"pageInfo\":{\"hasNextPage\":true,\"endCursor\":\"c1\"},\"nodes\":[{\"id\":\"1\",\"name\":\"A\",\"type\":\"USER_ACCOUNT\"},{\"id\":\"2\",\"name\":\"B\",\"type\":\"USER_ACCOUNT\"}]}}}",
                2 => "{\"data\":{\"cloudResourcesV2\":{\"pageInfo\":{\"hasNextPage\":false},\"nodes\":[{\"id\":\"3\",\"name\":\"C\",\"type\":\"USER_ACCOUNT\"},{\"id\":\"4\",\"name\":\"D\",\"type\":\"USER_ACCOUNT\"}]}}}",
                _ => "{\"data\":{\"cloudResourcesV2\":{\"pageInfo\":{\"hasNextPage\":false},\"nodes\":[]}}}"
            };

            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
        }
    }

    [TestMethod]
    public async Task GetAllUsersAsync_ReturnsAllPages() {
        var handler = new PagingHandler();
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var original = (HttpClient)field!.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token");
            var users = await client.GetAllUsersAsync(pageSize: 2, degreeOfParallelism: 2);
            Assert.AreEqual(4, users.Count);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, users.Select(u => u.Id).ToArray());
        } finally {
            field.SetValue(null, original);
        }
    }

    [TestMethod]
    public async Task GetUsersAsyncEnumerable_ReturnsAllPages() {
        var handler = new PagingHandler();
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var original = (HttpClient)field!.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token");
            var list = new List<WizUser>();
            await foreach (var user in client.GetUsersAsyncEnumerable(2, null, null, 2)) {
                list.Add(user);
            }
            Assert.AreEqual(4, list.Count);
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, list.Select(u => u.Id).ToArray());
        } finally {
            field.SetValue(null, original);
        }
    }

    [TestMethod]
    public async Task GetUsersAsyncEnumerable_PrefetchesNextPage() {
        var handler = new PagingHandler();
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var original = (HttpClient)field!.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token");
            await using var enumerator = client.GetUsersAsyncEnumerable(2, null, null, 2).GetAsyncEnumerator();
            Assert.IsTrue(await enumerator.MoveNextAsync());
            Assert.IsTrue(handler.CallCount >= 2);
        } finally {
            field.SetValue(null, original);
        }
    }
}

