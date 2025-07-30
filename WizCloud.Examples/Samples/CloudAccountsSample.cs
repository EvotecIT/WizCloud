using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class CloudAccountsSample {
    public static async Task RunAsync() {
        var token = await WizAuthentication.AcquireTokenAsync("clientId", "clientSecret");
        using var client = new WizClient(token);
        var accounts = await client.GetCloudAccountsAsync(pageSize: 1);
        Console.WriteLine($"Cloud accounts sample retrieved {accounts.Count} account(s).");
        if (accounts.Count > 0) {
            Console.WriteLine($"First account provider: {accounts[0].CloudProvider}");
        }
    }
}