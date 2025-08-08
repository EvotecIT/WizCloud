using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>Gets cloud resources from Wiz.io.</summary>
/// <para>Retrieves resource inventory items from the Wiz API.</para>
/// <list type="alertSet">
/// <item>
/// <description>Large result sets may require multiple API requests and increase run time.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get all resources</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizResource</code>
/// <para>Returns all resources visible to the current connection.</para>
/// </example>
/// <example>
/// <summary>Filter by type and page size</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizResource -Type vm -PageSize 100</code>
/// <para>Retrieves virtual machines 100 at a time.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizResource")]
[OutputType(typeof(WizResource))]
public class CmdletGetWizResource : AsyncPSCmdlet {
    /// <summary>The number of resources to retrieve per page. Default is 500.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of resources to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>Filter resources by type.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by resource type.")]
    public string[] Type { get; set; } = Array.Empty<string>();
    /// <summary>Filter resources by cloud provider.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by cloud provider.")]
    public WizCloudProvider[] CloudProvider { get; set; } = Array.Empty<WizCloudProvider>();
    /// <summary>Filter resources by region.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by resource region.")]
    public string? Region { get; set; }
    /// <summary>Filter resources by public accessibility.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by public accessibility.")]
    public SwitchParameter PubliclyAccessible { get; set; }
    /// <summary>Filter resources by tag.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by tags.")]
    public Hashtable? Tag { get; set; }
    /// <summary>Filter resources by project identifier.</summary>
    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }
    /// <summary>Maximum number of resources to retrieve. Default is unlimited.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of resources to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    private WizClient? _wizClient;
    private int _retrievedCount = 0;
    /// <summary>Initializes the Wiz client for resource retrieval.</summary>
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
    /// <summary>Processes the Get-WizResource command.</summary>
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
            WriteVerbose($"Retrieving Wiz resources with page size: {PageSize}" +
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizResource", "Retrieving resources from Wiz...");
            WriteProgress(progressRecord);

            IDictionary<string, string>? tagDict = null;
            if (Tag != null && Tag.Count > 0) {
                tagDict = new Dictionary<string, string>();
                foreach (DictionaryEntry entry in Tag) {
                    tagDict[entry.Key.ToString()!] = entry.Value?.ToString() ?? string.Empty;
                }
            }

            await foreach (var resource in _wizClient.GetResourcesAsyncEnumerable(
                PageSize,
                Type,
                CloudProvider,
                Region,
                PubliclyAccessible.IsPresent ? true : (bool?)null,
                tagDict,
                ProjectId,
                CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                WriteObject(resource);
                _retrievedCount++;

                if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} resources...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} resources...";
                    WriteProgress(progressRecord);
                }

                if (MaxResults.HasValue && _retrievedCount >= MaxResults.Value) {
                    WriteVerbose($"Reached maximum result limit of {MaxResults.Value} resources");
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
                "WizResourceRetrievalError",
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
