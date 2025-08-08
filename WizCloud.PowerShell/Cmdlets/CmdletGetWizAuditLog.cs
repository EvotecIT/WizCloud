using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>Gets audit logs from Wiz.io.</summary>
/// <para>Retrieves records of user actions and system events.</para>
/// <list type="alertSet">
/// <item>
/// <description>Audit logs may be extensive; apply date or user filters to narrow the output.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get recent audit logs</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizAuditLog</code>
/// <para>Returns audit logs using default paging.</para>
/// </example>
/// <example>
/// <summary>Filter by user and date</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizAuditLog -User admin@example.com -StartDate (Get-Date).AddDays(-1)</code>
/// <para>Retrieves actions for the specified user in the last day.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizAuditLog")]
[OutputType(typeof(WizAuditLogEntry))]
public class CmdletGetWizAuditLog : AsyncPSCmdlet {
    /// <summary>The number of audit logs to retrieve per page. Default is 500.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of audit logs to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>Filter audit logs by start date.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by start date.")]
    public DateTime? StartDate { get; set; }
    /// <summary>Filter audit logs by end date.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by end date.")]
    public DateTime? EndDate { get; set; }
    /// <summary>Filter audit logs by user.</summary>
    [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by user.")]
    public string? User { get; set; }
    /// <summary>Filter audit logs by action.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by action.")]
    public string? Action { get; set; }
    /// <summary>Filter audit logs by status.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by status.")]
    public string? Status { get; set; }
    /// <summary>Maximum number of audit logs to retrieve. Default is unlimited.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of audit logs to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;
    /// <summary>Initializes the Wiz client for audit log retrieval.</summary>
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
    /// <summary>Processes the Get-WizAuditLog command.</summary>
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
            WriteVerbose($"Retrieving Wiz audit logs with page size: {PageSize}" +
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizAuditLog", "Retrieving audit logs from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var log in _wizClient.GetAuditLogsAsyncEnumerable(PageSize, StartDate, EndDate, User, Action, Status, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(log);
                _retrievedCount++;

                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} audit logs...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} audit logs...";
                    WriteProgress(progressRecord);
                }

                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} audit logs");
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
                "WizAuditLogRetrievalError",
                ErrorCategory.ReadError,
                null));
        }
    }
    /// <summary>Releases the Wiz client resources.</summary>
    protected override Task EndProcessingAsync() {
        _wizClient?.Dispose();
        return Task.CompletedTask;
    }
}
