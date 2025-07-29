using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class MultiClientSample {
    public static async Task RunAsync() {
        var token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
        if (string.IsNullOrEmpty(token)) {
            Console.WriteLine("WIZ_SERVICE_ACCOUNT_TOKEN environment variable is not set.");
            return;
        }

        using var first = new WizClient(token);
        using var second = new WizClient(token);
        var users1 = await first.GetUsersAsync(pageSize: 1);
        var users2 = await second.GetUsersAsync(pageSize: 1);
        Console.WriteLine($"Multi-client sample retrieved {users1.Count + users2.Count} user(s) using a shared HttpClient instance.");
    }
}
