using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>Gets compliance posture from Wiz.io.</summary>
/// <para>Retrieves compliance scores for supported frameworks.</para>
/// <list type="alertSet">
/// <item>
/// <description>Results represent the latest assessment and may change as new scans run.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get compliance posture</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizCompliance</code>
/// <para>Returns compliance scores for all available frameworks.</para>
/// </example>
/// <example>
/// <summary>Filter by framework and score</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizCompliance -Framework CIS -MinScore 80</code>
/// <para>Shows CIS results with scores of at least 80.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizCompliance")]
[OutputType(typeof(WizComplianceResult))]
public class CmdletGetWizCompliance : AsyncPSCmdlet {
    /// <summary>Filter compliance results by framework.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by compliance framework.")]
    public string[] Framework { get; set; } = Array.Empty<string>();
    /// <summary>Filter results by minimum compliance score.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by minimum compliance score.")]
    public double? MinScore { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;
    /// <summary>Initializes the Wiz client for compliance retrieval.</summary>
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
    /// <summary>Processes the Get-WizCompliance command.</summary>
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
    /// <summary>Releases the Wiz client resources.</summary>
    protected override Task EndProcessingAsync() {
        _wizClient?.Dispose();
        return Task.CompletedTask;
    }
}
