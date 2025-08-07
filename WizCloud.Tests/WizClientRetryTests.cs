using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class WizClientRetryTests {
    private sealed class TransientStatusHandler : HttpMessageHandler {
        private readonly int _failures;
        public int CallCount { get; private set; }

        public TransientStatusHandler(int failures) => _failures = failures;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            CallCount++;
            if (CallCount <= _failures)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{\"data\":{}}")
            });
        }
    }

    private sealed class ExceptionHandler : HttpMessageHandler {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            CallCount++;
            throw new HttpRequestException("fail");
        }
    }

    [TestMethod]
    public async Task SendGraphQlRequestAsync_RetriesOnTransientStatusAndSucceeds() {
        var handler = new TransientStatusHandler(2);
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic)!;
        var original = (HttpClient)field.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token", retryCount: 2, retryDelay: TimeSpan.FromMilliseconds(1));
            var method = typeof(WizClient).GetMethod("SendGraphQlRequestAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var task = (Task<JsonNode>)method.Invoke(client, new object[] { new { query = "" } })!;
            var result = await task.ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, handler.CallCount);
        } finally {
            field.SetValue(null, original);
        }
    }

    [TestMethod]
    public async Task SendGraphQlRequestAsync_RetriesAndThrowsAfterMaxAttempts() {
        var handler = new ExceptionHandler();
        var field = typeof(WizClient).GetField("_httpClient", BindingFlags.Static | BindingFlags.NonPublic)!;
        var original = (HttpClient)field.GetValue(null)!;
        try {
            field.SetValue(null, new HttpClient(handler));
            using var client = new WizClient("token", retryCount: 2, retryDelay: TimeSpan.FromMilliseconds(1));
            var method = typeof(WizClient).GetMethod("SendGraphQlRequestAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => {
                var task = (Task<JsonNode>)method.Invoke(client, new object[] { new { query = "" } })!;
                await task.ConfigureAwait(false);
            }).ConfigureAwait(false);
            Assert.AreEqual(3, handler.CallCount);
        } finally {
            field.SetValue(null, original);
        }
    }
}
