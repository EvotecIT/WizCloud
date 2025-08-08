using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>Gets cloud accounts from Wiz.io.</summary>
/// <para>Enumerates cloud provider accounts linked to your organization.</para>
/// <list type="alertSet">
/// <item>
/// <description>Retrieving many accounts may trigger API rate limiting.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get all cloud accounts</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizCloudAccount</code>
/// <para>Lists every account accessible to the current connection.</para>
/// </example>
/// <example>
/// <summary>Limit the number of accounts</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizCloudAccount -MaxResults 50 -PageSize 50</code>
/// <para>Retrieves at most fifty accounts in pages of fifty.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizCloudAccount")]
[OutputType(typeof(WizCloudAccount))]
public class CmdletGetWizCloudAccount : AsyncPSCmdlet {
    /// <summary>The number of cloud accounts to retrieve per page. Default is 500.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of cloud accounts to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>The maximum number of cloud accounts to retrieve. Use this to limit results when dealing with large datasets.</summary>
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