using System.Threading.Tasks;
using WizCloud.Examples.Samples;

namespace WizCloud.Examples
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await TokenAuthSample.RunAsync();
            await ClientCredentialSample.RunAsync();
        }
    }
}