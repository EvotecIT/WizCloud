using System;
using System.Threading.Tasks;

namespace WizCloud.Examples;
internal static class GraphQlQueriesSample {
    public static Task RunAsync() {
        Console.WriteLine($"Users query length: {GraphQlQueries.UsersQuery.Length}");
        Console.WriteLine($"Projects query length: {GraphQlQueries.ProjectsQuery.Length}");
        Console.WriteLine($"Cloud accounts query length: {GraphQlQueries.CloudAccountsQuery.Length}");
        return Task.CompletedTask;
    }
}
