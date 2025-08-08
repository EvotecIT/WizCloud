using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>Gets network exposure data from Wiz.io.</summary>
/// <para>Retrieves open port and protocol exposure information.</para>
/// <list type="alertSet">
/// <item>
/// <description>Use port or protocol filters to limit the volume of returned data.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get all network exposures</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizNetworkExposure</code>
/// <para>Returns every network exposure record.</para>
/// </example>
/// <example>
/// <summary>Filter by port and protocol</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizNetworkExposure -Port 443 -Protocol tcp</code>
/// <para>Retrieves exposures for TCP port 443.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizNetworkExposure")]
[OutputType(typeof(WizNetworkExposure))]
public class CmdletGetWizNetworkExposure : AsyncPSCmdlet {
    /// <summary>The number of exposures to retrieve per page. Default is 500.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of exposures to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>Filter exposures by port.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by port.")]
    public int[] Port { get; set; } = Array.Empty<int>();
    /// <summary>Filter exposures by protocol.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by protocol.")]
    public string[] Protocol { get; set; } = Array.Empty<string>();
    /// <summary>Filter exposures by internet-facing status.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by internet facing status.")]
    public bool? InternetFacing { get; set; }
    /// <summary>Filter exposures by project identifier.</summary>
    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }
    /// <summary>Maximum number of exposures to retrieve. Default is unlimited.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of exposures to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;
    /// <summary>Initializes the Wiz client for network exposure retrieval.</summary>
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
    /// <summary>Processes the Get-WizNetworkExposure command.</summary>
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
            WriteVerbose($"Retrieving Wiz network exposure with page size: {PageSize}" +
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizNetworkExposure", "Retrieving network exposure from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var exposure in _wizClient.GetNetworkExposuresAsyncEnumerable(PageSize, Port, Protocol, InternetFacing, ProjectId, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(exposure);
                _retrievedCount++;

                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} exposures...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} exposures...";
                    WriteProgress(progressRecord);
                }

                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} exposures");
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
                "WizNetworkExposureRetrievalError",
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
