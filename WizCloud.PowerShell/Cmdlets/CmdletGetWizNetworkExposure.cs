using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;

/// <summary>
/// <para type="synopsis">Gets network exposure data from Wiz.io.</para>
/// <para type="description">The Get-WizNetworkExposure cmdlet retrieves network exposure information from Wiz.io using streaming enumeration.</para>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizNetworkExposure")]
[OutputType(typeof(WizNetworkExposure))]
public class CmdletGetWizNetworkExposure : AsyncPSCmdlet {
    /// <summary>
    /// <para type="description">The number of exposures to retrieve per page. Default is 500.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of exposures to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;

    /// <summary>
    /// <para type="description">Filter exposures by port.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by port.")]
    public int[] Port { get; set; } = Array.Empty<int>();

    /// <summary>
    /// <para type="description">Filter exposures by protocol.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by protocol.")]
    public string[] Protocol { get; set; } = Array.Empty<string>();

    /// <summary>
    /// <para type="description">Filter exposures by internet-facing status.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by internet facing status.")]
    public bool? InternetFacing { get; set; }

    /// <summary>
    /// <para type="description">Filter exposures by project identifier.</para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }

    /// <summary>
    /// <para type="description">Maximum number of exposures to retrieve. Default is unlimited.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of exposures to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;

    /// <summary>
    /// <para type="description">Initializes the Wiz client for network exposure retrieval.</para>
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
    /// <para type="description">Processes the Get-WizNetworkExposure command.</para>
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

    /// <summary>
    /// <para type="description">Releases the Wiz client resources.</para>
    /// </summary>
    protected override Task EndProcessingAsync() {
        _wizClient?.Dispose();
        return Task.CompletedTask;
    }
}
