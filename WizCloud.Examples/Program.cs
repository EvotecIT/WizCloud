using System.Threading.Tasks;
namespace WizCloud.Examples;
internal class Program {
    private static async Task Main(string[] args) {
        await TokenAuthSample.RunAsync();
        await ClientCredentialSample.RunAsync();
        await MultiClientSample.RunAsync();
        await StreamingSample.RunAsync();
        await UsersProgressSample.RunAsync();
        await ErrorHandlingSample.RunAsync();
        await GraphQlQueriesSample.RunAsync();
        await CloudAccountsSample.RunAsync();
        await FilterUsersSample.RunAsync();
    }
}