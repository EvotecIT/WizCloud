using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>Gets security issues from Wiz.io.</summary>
/// <para>Streams security issues reported by the Wiz platform.</para>
/// <list type="alertSet">
/// <item>
/// <description>Retrieving all issues may produce a large volume of output and take considerable time.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get all issues</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizIssue</code>
/// <para>Returns every issue available to the current connection.</para>
/// </example>
/// <example>
/// <summary>Filter by severity</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizIssue -Severity High</code>
/// <para>Retrieves only high-severity issues.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizIssue")]
[OutputType(typeof(WizIssue))]
public class CmdletGetWizIssue : AsyncPSCmdlet {
    /// <summary>The number of issues to retrieve per page. Default is 20.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of issues to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>Filter issues by severity.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by issue severities.")]
    public WizSeverity[] Severity { get; set; } = Array.Empty<WizSeverity>();
    /// <summary>Filter issues by status.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by issue status.")]
    public string[] Status { get; set; } = Array.Empty<string>();
    /// <summary>Filter issues by project identifier.</summary>
    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }
    /// <summary>Filter issues by type.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by issue types.")]
    public string[] Type { get; set; } = Array.Empty<string>();
    /// <summary>The maximum number of issues to retrieve. Default is unlimited.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of issues to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;

    /// <summary>
    /// Initialize the Wiz client.
    /// </summary>
    protected override Task BeginProcessingAsync() {
        try {
            var token = ModuleInitialization.DefaultToken;
            if (string.IsNullOrEmpty(token)) {
                WriteError(new ErrorRecord(
                    new InvalidOperationException("No authentication found. Please use Connect-Wiz first."),
                    "NoTokenAvailable",
                    ErrorCategory.AuthenticationError,
                    null));
                return Task.CompletedTask;
            }

            WriteVerbose("Using stored token from Connect-Wiz");
            var region = ModuleInitialization.DefaultRegion;
            WriteVerbose($"Using region from Connect-Wiz: {region}");

            var clientId = ModuleInitialization.DefaultClientId;
            var clientSecret = ModuleInitialization.DefaultClientSecret;

            _wizClient = !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret)
                ? new WizClient(token, region, clientId, clientSecret)
                : new WizClient(token, region);
            WriteVerbose($"Connected to Wiz region: {region}");
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
    /// Retrieve and output Wiz issues.
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
            WriteVerbose($"Retrieving Wiz issues with page size: {PageSize}" +
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizIssue", "Retrieving issues from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var issue in _wizClient.GetIssuesAsyncEnumerable(PageSize, Severity, Status, ProjectId, Type, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(issue);
                _retrievedCount++;

                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} issues...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} issues...";
                    WriteProgress(progressRecord);
                }

                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} issues");
                    break;
                }
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
                "WizIssueRetrievalError",
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
