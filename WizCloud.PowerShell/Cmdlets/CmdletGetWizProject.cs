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
/// <code>Get-WizProject</code>
/// </example>
/// <example>
/// <para>Get projects with a specific page size:</para>
/// <code>Get-WizProject -PageSize 100</code>
/// </example>
/// <example>
/// <para>Select a region using Connect-Wiz and then retrieve projects:</para>
/// <code>Connect-Wiz -Region "us1"; Get-WizProject</code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizProject")]
[OutputType(typeof(WizProject))]
public class CmdletGetWizProject : AsyncPSCmdlet {

    /// <summary>
    /// <para type="description">The number of projects to retrieve per page. Default is 20.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of projects to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;

    /// <summary>
    /// <para type="description">The maximum number of projects to retrieve. Use this to limit results when dealing with large datasets.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of projects to retrieve. Default is unlimited.")]
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

            var clientId = ModuleInitialization.DefaultClientId;
            var clientSecret = ModuleInitialization.DefaultClientSecret;

            var region = ModuleInitialization.DefaultRegion;
            WriteVerbose($"Using region: {region}");

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
            WriteVerbose($"Retrieving Wiz projects with page size: {PageSize}" + 
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizProject", "Retrieving projects from Wiz...");
            WriteProgress(progressRecord);

            await foreach (var project in _wizClient.GetProjectsAsyncEnumerable(PageSize, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(project);
                _retrievedCount++;

                // Update progress
                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} projects...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} projects...";
                    WriteProgress(progressRecord);
                }

                // Check if we've reached the maximum results
                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} projects");
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
