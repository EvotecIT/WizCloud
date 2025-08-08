using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class GetUsersWithProgressAsyncEnumerableTests {
    private sealed class SequenceHandler : HttpMessageHandler {
        private readonly Queue<string> _responses = new();
        public SequenceHandler() {
            _responses.Enqueue("{\"data\":{\"cloudResourcesV2\":{\"totalCount\":2}}}");
            _responses.Enqueue("{\"data\":{\"cloudResourcesV2\":{\"pageInfo\":{\"hasNextPage\":false},\"nodes\":[{\"id\":\"1\",\"name\":\"A\",\"type\":\"USER_ACCOUNT\"},{\"id\":\"2\",\"name\":\"B\",\"type\":\"USER_ACCOUNT\"}]}}}");
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            var content = _responses.Dequeue();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });
        }
    }

    [TestMethod]
    public async Task ReportsProgressAndRespectsMaxResults() {
        var handler = new SequenceHandler();
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var original = (HttpClient)field!.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token");
            var progressEvents = new List<WizProgress>();
            var progress = new TestProgress(p => progressEvents.Add(p));
            var users = new List<WizUser>();
            await foreach (var user in client.GetUsersWithProgressAsyncEnumerable(pageSize: 2, maxResults: 1, includeTotal: true, progress: progress)) {
                users.Add(user);
            }
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(2, progressEvents.Count);
            Assert.AreEqual(0, progressEvents[0].Retrieved);
            Assert.AreEqual(1, progressEvents[0].Total);
            Assert.AreEqual(1, progressEvents[1].Retrieved);
            Assert.AreEqual(1, progressEvents[1].Total);
        } finally {
            field.SetValue(null, original);
        }
    }

    private sealed class TestProgress : IProgress<WizProgress> {
        private readonly Action<WizProgress> _handler;

        public TestProgress(Action<WizProgress> handler) {
            _handler = handler;
        }

        public void Report(WizProgress value) {
            _handler(value);
        }
    }
}