using System;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Examples;
internal static class ClientCredentialSample {
    public static async Task RunAsync() {
        var clientId = Environment.GetEnvironmentVariable("WIZ_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("WIZ_CLIENT_SECRET");
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)) {
            Console.WriteLine("WIZ_CLIENT_ID or WIZ_CLIENT_SECRET environment variables are not set.");
            return;
        }

        var token = await WizAuthentication.AcquireTokenAsync(clientId, clientSecret);
        using var client = new WizClient(token);
        var users = await client.GetUsersAsync(pageSize: 1);
        Console.WriteLine($"Client credentials sample retrieved {users.Count} user(s).");
    }
}