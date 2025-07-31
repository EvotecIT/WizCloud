using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class FilterUsersSample {
    public static async Task RunAsync() {
        var token = await WizAuthentication.AcquireTokenAsync("clientId", "clientSecret");
        using var client = new WizClient(token);
        var users = await client.GetUsersAsync(types: new[] { WizUserType.SERVICE_ACCOUNT }, projectId: "project1");
        Console.WriteLine($"Filter sample retrieved {users.Count} user(s).");
        if (users.Count > 0) {
            Console.WriteLine($"First filtered user: {users[0].Name}");
        }
    }
}
