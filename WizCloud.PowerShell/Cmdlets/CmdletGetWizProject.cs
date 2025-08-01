using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>
/// <para type="synopsis">Gets projects from Wiz.io.</para>
/// <para type="description">The Get-WizProject cmdlet retrieves projects from Wiz.io using streaming enumeration.</para>
/// <example>
/// <para>Get all projects from Wiz:</para>
/// <code>Get-WizProject -Token $token</code>
/// </example>
/// <example>
/// <para>Get projects with a specific page size:</para>
/// <code>Get-WizProject -Token $token -PageSize 100</code>
/// </example>
/// <example>
/// <para>Get projects from a specific region:</para>
/// <code>Get-WizProject -Token $token -Region "us1"</code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizProject")]
[OutputType(typeof(WizProject))]
public class CmdletGetWizProject : AsyncPSCmdlet {
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
    public WizRegion? Region { get; set; }

    /// <summary>
    /// <para type="description">The number of projects to retrieve per page. Default is 20.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of projects to retrieve per page.")]
    [ValidateRange(1, 500)]
    public int PageSize { get; set; } = 20;

    private WizClient? _wizClient;

    /// <summary>
    /// Initialize the Wiz client.
    /// </summary>
    protected override Task BeginProcessingAsync() {
        try {
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

            if (Region is null) {
                Region = ModuleInitialization.DefaultRegion;
                WriteVerbose($"Using region: {Region}");
            }

            var clientId = ModuleInitialization.DefaultClientId;
            var clientSecret = ModuleInitialization.DefaultClientSecret;

            _wizClient = !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret)
                ? new WizClient(Token!, Region.Value, clientId, clientSecret)
                : new WizClient(Token!, Region.Value);
            WriteVerbose($"Connected to Wiz region: {Region}");
        } catch (HttpRequestException ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizApiHttpError",
                ErrorCategory.ConnectionError,
                null));
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
    /// Retrieve and output Wiz projects.
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
            WriteVerbose($"Retrieving Wiz projects with page size: {PageSize}");

            var progressRecord = new ProgressRecord(1, "Get-WizProject", "Retrieving projects from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var project in _wizClient.GetProjectsAsyncEnumerable(PageSize, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(project);
            }

            progressRecord.StatusDescription = "Completed";
            progressRecord.PercentComplete = 100;
            progressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progressRecord);
        } catch (HttpRequestException ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizApiHttpError",
                ErrorCategory.ReadError,
                null));
        } catch (Exception ex) {
            WriteError(new ErrorRecord(
                ex,
                "WizProjectRetrievalError",
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
