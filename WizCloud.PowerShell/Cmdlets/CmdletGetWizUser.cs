using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>
/// <para type="synopsis">Gets users from Wiz.io with all their properties.</para>
/// <para type="description">The Get-WizUser cmdlet retrieves users from Wiz.io including their security properties, projects, cloud accounts, and issue analytics. By default, returns enhanced user objects with all properties exposed. Use -Raw to get the original API response.</para>
/// <example>
/// <para>Select a region using Connect-Wiz and get all users from Wiz:</para>
/// <code>Connect-Wiz -Region "us1"; Get-WizUser</code>
/// </example>
/// <example>
/// <para>Select a region using Connect-Wiz and retrieve a limited number of users:</para>
/// <code>Connect-Wiz -Region "us1"; Get-WizUser -MaxResults 1000 -PageSize 500</code>
/// </example>
/// <example>
/// <para>Select a region using Connect-Wiz and get raw API response objects:</para>
/// <code>Connect-Wiz -Region "us1"; Get-WizUser -Raw -MaxResults 10</code>
/// </example>
/// <example>
/// <para>Select a region using Connect-Wiz and report progress including total counts:</para>
/// <code>Connect-Wiz -Region "us1"; Get-WizUser -IncludeTotal</code>
/// </example>
/// </summary>
[Cmdlet(VerbsCommon.Get, "WizUser")]
[OutputType(typeof(WizUserComprehensive), typeof(WizUser))]
public class CmdletGetWizUser : AsyncPSCmdlet {

    /// <summary>
    /// <para type="description">The number of users to retrieve per page. Default is 20.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of users to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;

    /// <summary>
    /// <para type="description">Filter users by Wiz user type.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by Wiz user types.")]
    public WizUserType[] Type { get; set; } = Array.Empty<WizUserType>();

    /// <summary>
    /// <para type="description">Filter users by project identifier.</para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }

    /// <summary>
    /// <para type="description">Maximum number of page requests to issue in parallel.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of page requests to issue in parallel.")]
    [ValidateRange(1, int.MaxValue)]
    public int Parallel { get; set; } = 1;

    /// <summary>
    /// <para type="description">The maximum number of users to retrieve. Use this to limit results when dealing with large datasets.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of users to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }

    /// <summary>
    /// <para type="description">Return raw API response objects without additional property expansion.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Return raw API response objects.")]
    public SwitchParameter Raw { get; set; }

    /// <summary>
    /// <para type="description">Include total user count in progress output. This requires an additional API call.</para>
    /// </summary>
    [Parameter(Mandatory = false, HelpMessage = "Include total user count in progress output.")]
    public SwitchParameter IncludeTotal { get; set; }

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
    /// Retrieve and output Wiz users.
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
            WriteVerbose($"Retrieving Wiz users with page size: {PageSize}" +
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : "") +
                (Parallel > 1 ? $", parallel: {Parallel}" : ""));

            var progressRecord = new ProgressRecord(1, "Get-WizUser", "Retrieving users from Wiz...");
            int? totalUsers = null;
            if (IncludeTotal) {
                totalUsers = await _wizClient.GetUsersCountAsync(Type, ProjectId).ConfigureAwait(false);
                var totalProp = progressRecord.GetType().GetProperty("Total");
                totalProp?.SetValue(progressRecord, totalUsers);
                progressRecord.StatusDescription = $"Retrieved 0 of {totalUsers} users...";
            }
            WriteProgress(progressRecord);

            var effectiveMax = MaxResults.HasValue && totalUsers.HasValue
                ? Math.Min(MaxResults.Value, totalUsers.Value)
                : MaxResults;

            await foreach (var user in _wizClient.GetUsersAsyncEnumerable(PageSize, Type, ProjectId, Parallel, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                // Return raw or enhanced object based on parameter
                if (Raw) {
                    WriteObject(user);
                } else {
                    // Return comprehensive typed object with all properties
                    var comprehensiveUser = WizUserComprehensive.FromWizUser(user);
                    WriteObject(comprehensiveUser);
                }
                _retrievedCount++;

                // Update progress
                if (totalUsers.HasValue) {
                    var total = totalUsers.Value;
                    if (effectiveMax.HasValue)
                        total = effectiveMax.Value;
                    var percentComplete = total > 0 ? (int)((double)_retrievedCount / total * 100) : 0;
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {total} users...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (MaxResults.HasValue) {
                    var percentComplete = (int)((double)_retrievedCount / MaxResults.Value * 100);
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} of {MaxResults.Value} users...";
                    progressRecord.PercentComplete = percentComplete;
                    WriteProgress(progressRecord);
                } else if (_retrievedCount % 100 == 0) {
                    progressRecord.StatusDescription = $"Retrieved {_retrievedCount} users...";
                    WriteProgress(progressRecord);
                }

                // Check if we've reached the maximum results
                if (effectiveMax.HasValue && _retrievedCount >= effectiveMax.Value) {
                    WriteVerbose($"Reached maximum result limit of {effectiveMax.Value} users");
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
                "WizUserRetrievalError",
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