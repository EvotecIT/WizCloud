using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class ErrorHandlingSample {
    public static async Task RunAsync() {
        Console.WriteLine("Running error handling sample...");
        using var client = new WizClient("invalid-token");
        try {
            await client.GetUsersAsync(pageSize: 1);
        } catch (HttpRequestException ex) {
            Console.WriteLine($"Expected failure: {ex.Message}");
        }
    }
}
