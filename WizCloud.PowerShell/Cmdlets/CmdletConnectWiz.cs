using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>Connects to Wiz.io and stores authentication for the session.</summary>
/// <para>Establishes a connection using a token or client credentials and optionally tests the API.</para>
/// <list type="alertSet">
/// <item>
/// <description>Stored credentials remain in memory until <c>Disconnect-Wiz</c> is executed.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Connect using a token</summary>
/// <code><prefix>PS&gt; </prefix>Connect-Wiz -Token 'your-service-account-token'</code>
/// <para>The token is cached for subsequent cmdlets.</para>
/// </example>
/// <example>
/// <summary>Connect with client credentials</summary>
/// <code><prefix>PS&gt; </prefix>Connect-Wiz -ClientId 'id' -ClientSecret 'secret' -Region us1</code>
/// <para>A token is acquired for region <c>us1</c>.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommunications.Connect, "Wiz")]
[OutputType(typeof(bool))]
public class CmdletConnectWiz : AsyncPSCmdlet {
    private const string TokenParameterSet = nameof(TokenParameterSet);
    private const string ClientCredentialParameterSet = nameof(ClientCredentialParameterSet);
    /// <summary>The Wiz service account token for authentication.</summary>
    [Parameter(Mandatory = true, Position = 0, ParameterSetName = TokenParameterSet, HelpMessage = "The Wiz service account token for authentication.")]
    [ValidateNotNullOrEmpty]
    public string? Token { get; set; }

    /// <summary>The service account client ID.</summary>
    [Parameter(Mandatory = true, ParameterSetName = ClientCredentialParameterSet, HelpMessage = "The Wiz service account client ID.")]
    [ValidateNotNullOrEmpty]
    public string? ClientId { get; set; }

    /// <summary>The service account client secret.</summary>
    [Parameter(Mandatory = true, ParameterSetName = ClientCredentialParameterSet, HelpMessage = "The Wiz service account client secret.")]
    [ValidateNotNullOrEmpty]
    public string? ClientSecret { get; set; }

    /// <summary>The Wiz region to connect to. Default is 'eu17'.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The Wiz region to connect to (e.g., 'eu17', 'us1', 'us2').")]
    public WizRegion Region { get; set; } = WizRegion.EU17;

    /// <summary>Tests the connection to Wiz.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Test the connection to Wiz.")]
    public SwitchParameter TestConnection { get; set; }

    /// <summary>Suppresses informational output and returns only a Boolean result.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Suppress the output messages.")]
    public SwitchParameter Suppress { get; set; }

    /// <summary>
    /// Processes the Connect-Wiz command asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ProcessRecordAsync() {
        try {
            if (ParameterSetName == ClientCredentialParameterSet) {
                WriteVerbose("Retrieving token using client credentials");
                Token = await WizAuthentication.AcquireTokenAsync(ClientId!, ClientSecret!, Region);
                WriteVerbose("Token acquired using client credentials");
            }

            // Store the credentials in the module state
            ModuleInitialization.DefaultToken = Token;
            ModuleInitialization.DefaultClientId = ParameterSetName == ClientCredentialParameterSet ? ClientId : null;
            ModuleInitialization.DefaultClientSecret = ParameterSetName == ClientCredentialParameterSet ? ClientSecret : null;
            ModuleInitialization.DefaultRegion = Region;

            // Test the connection if requested
            if (TestConnection) {
                WriteVerbose($"Testing connection to Wiz region: {Region}");

                using var testClient = new WizClient(Token!, Region);
                
                // Just test if we can make a successful API call by checking for projects
                // We use GetProjectsAsyncEnumerable with immediate break to avoid fetching all data
                await foreach (var _ in testClient.GetProjectsAsyncEnumerable(1)) {
                    break; // Exit after first project to avoid loading all data
                }
                
                if (!Suppress) {
                    WriteInformation($"Successfully connected to Wiz region '{Region}'. API connection verified.", new string[] { "WizConnect" });
                }
            } else {
                if (!Suppress) {
                    WriteInformation($"Connected to Wiz region '{Region}'. Token stored for session.", new string[] { "WizConnect" });
                }
            }

            WriteObject(true);
        } catch (HttpRequestException ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizApiHttpError",
                ErrorCategory.ConnectionError,
                null));
            WriteObject(false);
        } catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizConnectionError",
                ErrorCategory.ConnectionError,
                null));
            WriteObject(false);
        }
    }
}