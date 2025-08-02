using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>
/// <para type="synopsis">Gets compliance posture from Wiz.io.</para>
/// <para type="description">The Get-WizCompliance cmdlet retrieves compliance posture results from Wiz.io.</para>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizCompliance")]
[OutputType(typeof(WizComplianceResult))]
public class CmdletGetWizCompliance : AsyncPSCmdlet {
    [Parameter(Mandatory = false, HelpMessage = "Filter by compliance framework.")]
    public string[] Framework { get; set; } = Array.Empty<string>();

    [Parameter(Mandatory = false, HelpMessage = "Filter by minimum compliance score.")]
    public double? MinScore { get; set; }

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
            WriteVerbose("Retrieving Wiz compliance posture");

            var progressRecord = new ProgressRecord(1, "Get-WizCompliance", "Retrieving compliance posture from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var result in _wizClient.GetCompliancePostureAsyncEnumerable(Framework, MinScore, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(result);
                _retrievedCount++;

                if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} compliance results...";
                    WriteProgress(progressRecord);
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
                "WizComplianceRetrievalError",
                ErrorCategory.ReadError,
                null));
        }
    }

    protected override Task EndProcessingAsync() {
        _wizClient?.Dispose();
        return Task.CompletedTask;
    }
}
