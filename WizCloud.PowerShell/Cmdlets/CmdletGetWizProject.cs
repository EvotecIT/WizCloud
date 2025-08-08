using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>Gets projects from Wiz.io.</summary>
/// <para>Retrieves project and folder information from the Wiz API.</para>
/// <list type="alertSet">
/// <item>
/// <description>Projects are returned in pages and may include folder entries.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get all projects</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizProject</code>
/// <para>Returns every project for the connected organization.</para>
/// </example>
/// <example>
/// <summary>Specify page size</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizProject -PageSize 100</code>
/// <para>Retrieves projects in pages of one hundred.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizProject")]
[OutputType(typeof(WizProject))]
public class CmdletGetWizProject : AsyncPSCmdlet {
    /// <summary>The number of projects to retrieve per page. Default is 20.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of projects to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>The maximum number of projects to retrieve. Use this to limit results when dealing with large datasets.</summary>
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
