using System;
using System.Management.Automation;
using System.Threading.Tasks;
using ISUID.PowerShell;
using WizCloud;

namespace WizCloud.PowerShell.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Connects to Wiz.io and stores authentication for the session.</para>
    /// <para type="description">The Connect-Wiz cmdlet establishes a connection to Wiz.io and stores the authentication token for use in other Wiz cmdlets during the session.</para>
    /// <example>
    /// <para>Connect using a token:</para>
    /// <code>Connect-Wiz -Token "your-service-account-token"</code>
    /// </example>
    /// <example>
    /// <para>Connect to a specific region:</para>
    /// <code>Connect-Wiz -Token "your-service-account-token" -Region "us1"</code>
    /// </example>
    /// <example>
    /// <para>Connect using environment variable:</para>
    /// <code>$env:WIZ_SERVICE_ACCOUNT_TOKEN = "your-token"
    /// Connect-Wiz</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommunications.Connect, "Wiz")]
    [OutputType(typeof(bool))]
    public class CmdletConnectWiz : AsyncPSCmdlet
    {
        /// <summary>
        /// <para type="description">The Wiz service account token for authentication.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, HelpMessage = "The Wiz service account token for authentication.")]
        [ValidateNotNullOrEmpty]
        public string? Token { get; set; }

        /// <summary>
        /// <para type="description">The Wiz region to connect to. Default is 'eu17'.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The Wiz region to connect to (e.g., 'eu17', 'us1', 'us2').")]
        [ValidateNotNullOrEmpty]
        public string Region { get; set; } = "eu17";

        /// <summary>
        /// <para type="description">Test the connection to Wiz.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Test the connection to Wiz.")]
        public SwitchParameter TestConnection { get; set; }

        /// <summary>
        /// Processes the Connect-Wiz command asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ProcessRecordAsync()
        {
            try
            {
                // If no token provided, check environment variable
                if (string.IsNullOrEmpty(Token))
                {
                    Token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
                    if (string.IsNullOrEmpty(Token))
                    {
                        WriteError(new ErrorRecord(
                            new ArgumentException("No token provided and WIZ_SERVICE_ACCOUNT_TOKEN environment variable is not set."),
                            "NoTokenProvided",
                            ErrorCategory.AuthenticationError,
                            null));
                        return;
                    }
                    WriteVerbose("Using token from WIZ_SERVICE_ACCOUNT_TOKEN environment variable");
                }

                // Store the credentials in the module state
                ModuleInitialization.DefaultToken = Token;
                ModuleInitialization.DefaultRegion = Region;

                // Test the connection if requested
                if (TestConnection)
                {
                    WriteVerbose($"Testing connection to Wiz region: {Region}");

                    using var testClient = new WizClient(Token!, Region);
                    var users = await testClient.GetUsersAsync(1); // Get just 1 user to test

                    WriteObject($"Successfully connected to Wiz region '{Region}'. Found {(users.Count > 0 ? "at least 1 user" : "no users")}.");
                }
                else
                {
                    WriteObject($"Connected to Wiz region '{Region}'. Token stored for session.");
                }

                WriteObject(true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "WizConnectionError",
                    ErrorCategory.ConnectionError,
                    null));
                WriteObject(false);
            }
        }
    }
}