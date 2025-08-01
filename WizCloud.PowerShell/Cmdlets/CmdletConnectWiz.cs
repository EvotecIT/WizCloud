using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>
/// <para type="synopsis">Connects to Wiz.io and stores authentication for the session.</para>
/// <para type="description">The Connect-Wiz cmdlet establishes a connection to Wiz.io and stores the authentication token for use in other Wiz cmdlets during the session. Returns true on success, false on failure.</para>
/// <example>
/// <para>Connect using a token:</para>
/// <code>Connect-Wiz -Token "your-service-account-token"</code>
/// </example>
/// <example>
/// <para>Connect to a specific region:</para>
/// <code>Connect-Wiz -Token "your-service-account-token" -Region "us1"</code>
/// </example>
/// <example>
/// <para>Connect using client credentials:</para>
/// <code>Connect-Wiz -ClientId "id" -ClientSecret "secret"</code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommunications.Connect, "Wiz")]
[OutputType(typeof(bool))]
public class CmdletConnectWiz : AsyncPSCmdlet {
    private const string TokenParameterSet = nameof(TokenParameterSet);
    private const string ClientCredentialParameterSet = nameof(ClientCredentialParameterSet);
    /// <summary>
    /// <para type="description">The Wiz service account token for authentication.</para>
    /// </summary>
    [Parameter(Mandatory = true, Position = 0, ParameterSetName = TokenParameterSet, HelpMessage = "The Wiz service account token for authentication.")]
    [ValidateNotNullOrEmpty]
    public string? Token { get; set; }

    /// <summary>
    /// <para type="description">The service account client ID.</para>
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = ClientCredentialParameterSet, HelpMessage = "The Wiz service account client ID.")]
    [ValidateNotNullOrEmpty]
    public string? ClientId { get; set; }

    /// <summary>
    /// <para type="description">The service account client secret.</para>
    /// </summary>
    [Parameter(Mandatory = true, ParameterSetName = ClientCredentialParameterSet, HelpMessage = "The Wiz service account client secret.")]
    [ValidateNotNullOrEmpty]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// <para type="description">The Wiz region to connect to. Default is 'eu17'.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The Wiz region to connect to (e.g., 'eu17', 'us1', 'us2').")]
    public WizRegion Region { get; set; } = WizRegion.EU17;

    /// <summary>
    /// <para type="description">Test the connection to Wiz.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Test the connection to Wiz.")]
    public SwitchParameter TestConnection { get; set; }

    /// <summary>
    /// <para type="description">Suppress the output (returns only true/false).</para>
    /// </summary>
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
                var hasProjects = false;
                await foreach (var project in testClient.GetProjectsAsyncEnumerable(1)) {
                    hasProjects = true;
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