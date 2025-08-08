using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class UsersProgressSample {
    public static async Task RunAsync() {
        var token = await WizAuthentication.AcquireTokenAsync("clientId", "clientSecret");
        using var client = new WizClient(token);
        var progress = new Progress<WizProgress>(p => {
            if (p.Total.HasValue) {
                Console.WriteLine($"Retrieved {p.Retrieved} of {p.Total} users...");
            } else {
                Console.WriteLine($"Retrieved {p.Retrieved} users...");
            }
        });
        await foreach (var user in client.GetUsersWithProgressAsyncEnumerable(pageSize: 1, maxResults: 2, includeTotal: true, progress: progress)) {
            Console.WriteLine($"User: {user.Name}");
        }
    }
}