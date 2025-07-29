using System;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Examples;
internal static class TokenAuthSample {
    public static async Task RunAsync() {
        var token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
        if (string.IsNullOrEmpty(token)) {
            Console.WriteLine("WIZ_SERVICE_ACCOUNT_TOKEN environment variable is not set.");
            return;
        }

        using var client = new WizClient(token);
        var users = await client.GetUsersAsync(pageSize: 1);
        Console.WriteLine($"Token auth sample retrieved {users.Count} user(s).");
        if (users.Count > 0) {
            Console.WriteLine($"First user type: {users[0].Type}");
        }

        var projects = await client.GetProjectsAsync(pageSize: 1);
        Console.WriteLine($"Token auth sample retrieved {projects.Count} project(s).");
        if (projects.Count > 0) {
            Console.WriteLine($"First project slug: {projects[0].Slug}");
        }
    }
}