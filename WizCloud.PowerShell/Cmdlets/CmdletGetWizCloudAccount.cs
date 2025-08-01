using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>
/// <para type="synopsis">Gets cloud accounts from Wiz.io.</para>
/// <para type="description">The Get-WizCloudAccount cmdlet retrieves cloud accounts from Wiz.io including AWS, Azure, and GCP accounts.</para>
/// <example>
/// <para>Get all cloud accounts from Wiz:</para>
/// <code>Get-WizCloudAccount</code>
/// </example>
/// <example>
/// <para>Get limited number of cloud accounts:</para>
/// <code>Get-WizCloudAccount -MaxResults 50 -PageSize 50</code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizCloudAccount")]
[OutputType(typeof(WizCloudAccount))]
public class CmdletGetWizCloudAccount : AsyncPSCmdlet {
    /// <summary>
    /// <para type="description">The number of cloud accounts to retrieve per page. Default is 500.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of cloud accounts to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;

    /// <summary>
    /// <para type="description">The maximum number of cloud accounts to retrieve. Use this to limit results when dealing with large datasets.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of cloud accounts to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;

    /// <summary>
    /// Initialize the Wiz client.
    /// </summary>
    protected override Task BeginProcessingAsync() {
        try {
            // Use stored token from Connect-Wiz
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

            // Use stored region from Connect-Wiz
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
    /// Retrieve and output Wiz cloud accounts.
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
            WriteVerbose($"Retrieving Wiz cloud accounts with page size: {PageSize}" + 
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizCloudAccount", "Retrieving cloud accounts from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var account in _wizClient.GetCloudAccountsAsyncEnumerable(PageSize, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(account);
                _retrievedCount++;

                // Update progress
                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} cloud accounts...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 10 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} cloud accounts...";
                    WriteProgress(progressRecord);
                }

                // Check if we've reached the maximum results
                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} cloud accounts");
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
                "WizCloudAccountRetrievalError",
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