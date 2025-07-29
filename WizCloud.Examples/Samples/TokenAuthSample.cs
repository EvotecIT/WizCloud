using System;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Examples.Samples
{
    internal static class TokenAuthSample
    {
        public static async Task RunAsync()
        {
            var token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("WIZ_SERVICE_ACCOUNT_TOKEN environment variable is not set.");
                return;
            }

            using var client = new WizClient(token);
            var users = await client.GetUsersAsync(pageSize: 1);
            Console.WriteLine($"Token auth sample retrieved {users.Count} user(s).");
        }
    }
}