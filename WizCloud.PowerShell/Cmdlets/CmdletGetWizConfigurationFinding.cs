using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>
/// <para type="synopsis">Gets configuration findings from Wiz.io.</para>
/// <para type="description">The Get-WizConfigurationFinding cmdlet retrieves configuration assessment findings from Wiz.io using streaming enumeration.</para>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizConfigurationFinding")]
[OutputType(typeof(WizConfigurationFinding))]
public class CmdletGetWizConfigurationFinding : AsyncPSCmdlet {
    /// <summary>
    /// <para type="description">The number of findings to retrieve per page. Default is 500.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of findings to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;

    [Parameter(Mandatory = false, HelpMessage = "Filter by compliance framework.")]
    public string[] Framework { get; set; } = Array.Empty<string>();

    [Parameter(Mandatory = false, HelpMessage = "Filter by severity.")]
    public WizSeverity[] Severity { get; set; } = Array.Empty<WizSeverity>();

    [Parameter(Mandatory = false, HelpMessage = "Filter by category.")]
    public string[] Category { get; set; } = Array.Empty<string>();

    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }

    [Parameter(Mandatory = false, HelpMessage = "Maximum number of findings to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;

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
            WriteVerbose($"Retrieving Wiz configuration findings with page size: {PageSize}" +
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizConfigurationFinding", "Retrieving configuration findings from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var finding in _wizClient.GetConfigurationFindingsAsyncEnumerable(PageSize, Framework, Severity, Category, ProjectId, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(finding);
                _retrievedCount++;

                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} findings...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} findings...";
                    WriteProgress(progressRecord);
                }

                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} findings");
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
                "WizConfigurationFindingRetrievalError",
                ErrorCategory.ReadError,
                null));
        }
    }

    protected override Task EndProcessingAsync() {
        _wizClient?.Dispose();
        return Task.CompletedTask;
    }
}
