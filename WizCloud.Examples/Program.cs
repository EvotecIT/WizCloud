using System;
using System.Threading.Tasks;
using WizCloud;
using WizCloud.Models;

namespace WizCloud.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Get the token from environment variable or replace with your actual token
            var token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
            
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Please set the WIZ_SERVICE_ACCOUNT_TOKEN environment variable");
                return;
            }

            try
            {
                // Create a new WizClient instance
                // Default region is "eu17", you can specify others like "us1", "us2", etc.
                using var wizClient = new WizClient(token, "eu17");
                
                Console.WriteLine("Fetching Wiz users...");
                
                // Get all users with their properties
                var users = await wizClient.GetUsersAsync(pageSize: 50);
                
                Console.WriteLine($"\nFound {users.Count} users:");
                Console.WriteLine(new string('-', 80));
                
                foreach (var user in users)
                {
                    Console.WriteLine($"\nUser: {user.Name}");
                    Console.WriteLine($"  ID: {user.Id}");
                    Console.WriteLine($"  Type: {user.Type}");
                    Console.WriteLine($"  Native Type: {user.NativeType}");
                    Console.WriteLine($"  Deleted: {(user.DeletedAt.HasValue ? user.DeletedAt.Value.ToString() : "No")}");
                    
                    // Security flags
                    Console.WriteLine($"\n  Security Properties:");
                    Console.WriteLine($"    Has Admin Privileges: {user.HasAdminPrivileges}");
                    Console.WriteLine($"    Has High Privileges: {user.HasHighPrivileges}");
                    Console.WriteLine($"    Has Access to Sensitive Data: {user.HasAccessToSensitiveData}");
                    Console.WriteLine($"    Has Sensitive Data: {user.HasSensitiveData}");
                    
                    // Projects
                    if (user.Projects?.Count > 0)
                    {
                        Console.WriteLine($"\n  Projects ({user.Projects.Count}):");
                        foreach (var project in user.Projects)
                        {
                            Console.WriteLine($"    - {project.Name} ({project.Id})");
                        }
                    }
                    
                    // Cloud Account
                    if (user.CloudAccount != null)
                    {
                        Console.WriteLine($"\n  Cloud Account:");
                        Console.WriteLine($"    Provider: {user.CloudAccount.CloudProvider}");
                        Console.WriteLine($"    Account: {user.CloudAccount.Name}");
                        Console.WriteLine($"    External ID: {user.CloudAccount.ExternalId}");
                    }
                    
                    // Issue Analytics
                    if (user.IssueAnalytics != null)
                    {
                        Console.WriteLine($"\n  Issue Analytics:");
                        Console.WriteLine($"    Total Issues: {user.IssueAnalytics.IssueCount}");
                        Console.WriteLine($"    Critical: {user.IssueAnalytics.CriticalSeverityCount}");
                        Console.WriteLine($"    High: {user.IssueAnalytics.HighSeverityCount}");
                        Console.WriteLine($"    Medium: {user.IssueAnalytics.MediumSeverityCount}");
                        Console.WriteLine($"    Low: {user.IssueAnalytics.LowSeverityCount}");
                        Console.WriteLine($"    Informational: {user.IssueAnalytics.InformationalSeverityCount}");
                    }
                    
                    Console.WriteLine(new string('-', 80));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}