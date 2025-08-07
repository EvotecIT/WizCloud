using System;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Examples;
internal static class ClientCredentialSample {
    public static async Task RunAsync() {
        using var client = await WizClient.CreateAsync("clientId", "clientSecret", retryCount: 5, retryDelay: TimeSpan.FromSeconds(2));
        var users = await client.GetUsersAsync(pageSize: 1);
        Console.WriteLine($"Client credentials sample retrieved {users.Count} user(s).");
        if (users.Count > 0) {
            Console.WriteLine($"First user type: {users[0].Type}");
        }

        var projects = await client.GetProjectsAsync(pageSize: 1);
        Console.WriteLine($"Client credentials sample retrieved {projects.Count} project(s).");
        if (projects.Count > 0) {
            Console.WriteLine($"First project slug: {projects[0].Slug}");
        }
    }
}