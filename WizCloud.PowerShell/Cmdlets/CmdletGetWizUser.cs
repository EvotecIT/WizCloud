using System;
using System.Management.Automation;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>
/// <para type="synopsis">Gets users from Wiz.io with all their properties.</para>
/// <para type="description">The Get-WizUser cmdlet retrieves users from Wiz.io including their security properties, projects, cloud accounts, and issue analytics.</para>
/// <example>
/// <para>Get all users from Wiz:</para>
/// <code>Get-WizUser -Token $token</code>
/// </example>
/// <example>
/// <para>Get users with a specific page size:</para>
/// <code>Get-WizUser -Token $token -PageSize 100</code>
/// </example>
/// <example>
/// <para>Get users from a specific region:</para>
/// <code>Get-WizUser -Token $token -Region "us1"</code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizUser")]
[OutputType(typeof(WizUser))]
public class CmdletGetWizUser : AsyncPSCmdlet {
    /// <summary>
    /// <para type="description">The Wiz service account token for authentication. If not provided, uses the token from Connect-Wiz.</para>
    /// </summary>
    [Parameter(Mandatory = false, Position = 0, HelpMessage = "The Wiz service account token for authentication.")]
    [ValidateNotNullOrEmpty]
    public string? Token { get; set; }

    /// <summary>
    /// <para type="description">The Wiz region to connect to. If not provided, uses the region from Connect-Wiz or defaults to 'eu17'.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The Wiz region to connect to (e.g., 'eu17', 'us1', 'us2').")]
    [ValidateNotNullOrEmpty]
    public string? Region { get; set; }

    /// <summary>
    /// <para type="description">The number of users to retrieve per page. Default is 20.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of users to retrieve per page.")]
    [ValidateRange(1, 500)]
    public int PageSize { get; set; } = 20;

    private WizClient? _wizClient;

    /// <summary>
    /// Initialize the Wiz client.
    /// </summary>
    protected override Task BeginProcessingAsync() {
        try {
            // Use stored token if not provided
            if (string.IsNullOrEmpty(Token)) {
                Token = ModuleInitialization.DefaultToken;
                if (string.IsNullOrEmpty(Token)) {
                    WriteError(new ErrorRecord(
                        new InvalidOperationException("No token provided. Please use Connect-Wiz first or provide a token parameter."),
                        "NoTokenAvailable",
                        ErrorCategory.AuthenticationError,
                        null));
                    return Task.CompletedTask;
                }
                WriteVerbose("Using stored token from Connect-Wiz");
            }

            // Use stored region if not provided
            if (string.IsNullOrEmpty(Region)) {
                Region = ModuleInitialization.DefaultRegion ?? "eu17";
                WriteVerbose($"Using region: {Region}");
            }

            _wizClient = new WizClient(Token!, Region ?? "eu17");
            WriteVerbose($"Connected to Wiz region: {Region}");
        } catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizClientInitializationError",
                ErrorCategory.ConnectionError,
                null));
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieve and output Wiz users.
    /// </summary>
    protected override async Task ProcessRecordAsync() {
        if (_wizClient == null) {
            WriteError(new ErrorRecord(
                new InvalidOperationException("Wiz client is not initialized"),
                "WizClientNotInitialized",
                ErrorCategory.InvalidOperation,
                null));
            return;
        }

        try {
            WriteVerbose($"Retrieving Wiz users with page size: {PageSize}");

            var progressRecord = new ProgressRecord(1, "Get-WizUser", "Retrieving users from Wiz...");
            WriteProgress(progressRecord);

            var users = await _wizClient.GetUsersAsync(PageSize);

            progressRecord.StatusDescription = $"Retrieved {users.Count} users";
            progressRecord.PercentComplete = 100;
            progressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progressRecord);

            WriteVerbose($"Successfully retrieved {users.Count} users");

            // Output each user to the pipeline
            foreach (var user in users) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(user);
            }
        } catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizUserRetrievalError",
                ErrorCategory.ReadError,
                null));
        }
    }

    /// <summary>
    /// Clean up resources.
    /// </summary>
    protected override Task EndProcessingAsync() {
        _wizClient?.Dispose();
        return Task.CompletedTask;
    }
}