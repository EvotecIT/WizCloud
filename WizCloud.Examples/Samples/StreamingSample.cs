using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class StreamingSample {
    public static async Task RunAsync() {
        var token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
        if (string.IsNullOrEmpty(token)) {
            Console.WriteLine("WIZ_SERVICE_ACCOUNT_TOKEN environment variable is not set.");
            return;
        }

        using var client = new WizClient(token);
        await foreach (var user in client.GetUsersAsyncEnumerable(pageSize: 1)) {
            Console.WriteLine($"Streaming user: {user.Name}");
            break;
        }

        await foreach (var project in client.GetProjectsAsyncEnumerable(pageSize: 1)) {
            Console.WriteLine($"Streaming project: {project.Name}");
            break;
        }
    }
}