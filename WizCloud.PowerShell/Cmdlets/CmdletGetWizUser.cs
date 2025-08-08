using System;
using System.Management.Automation;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.PowerShell;
/// <summary>Gets users from Wiz.io.</summary>
/// <para>Retrieves user identities along with security properties and related projects.</para>
/// <list type="alertSet">
/// <item>
/// <description>Using <c>-Raw</c> returns original API objects and may consume additional memory.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Get all users</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizUser</code>
/// <para>Retrieves enhanced user objects for the current connection.</para>
/// </example>
/// <example>
/// <summary>Limit results and return raw objects</summary>
/// <code><prefix>PS&gt; </prefix>Get-WizUser -MaxResults 10 -Raw</code>
/// <para>Outputs the first ten users using the raw API response.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommon.Get, "WizUser")]
[OutputType(typeof(WizUserComprehensive), typeof(WizUser))]
public class CmdletGetWizUser : AsyncPSCmdlet {
    /// <summary>The number of users to retrieve per page. Default is 20.</summary>
    [Parameter(Mandatory = false, HelpMessage = "The number of users to retrieve per page.")]
    [ValidateRange(1, 5000)]
    public int PageSize { get; set; } = 500;
    /// <summary>Filter users by Wiz user type.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Filter by Wiz user types.")]
    public WizUserType[] Type { get; set; } = Array.Empty<WizUserType>();
    /// <summary>Filter users by project identifier.</summary>
    [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Filter by project identifier.")]
    public string? ProjectId { get; set; }
    /// <summary>The maximum number of users to retrieve. Use this to limit results when dealing with large datasets.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Maximum number of users to retrieve. Default is unlimited.")]
    [ValidateRange(1, int.MaxValue)]
    public int? MaxResults { get; set; }
    /// <summary>Return raw API response objects without additional property expansion.</summary>
    [Parameter(Mandatory = false, HelpMessage = "Return raw API response objects.")]
    public SwitchParameter Raw { get; set; }

    private WizClient? _wizClient;

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
                (MaxResults.HasValue ? $", max results: {MaxResults.Value}" : string.Empty));

            var progressRecord = new ProgressRecord(1, "Get-WizUser", "Retrieving users from Wiz...");
            var totalProp = progressRecord.GetType().GetProperty("Total");

            var progress = new Progress<WizProgress>(info => {
                totalProp?.SetValue(progressRecord, info.Total);
                var total = info.Total ?? 0;
                var percentComplete = total > 0 ? (int)((double)info.Retrieved / total * 100) : 0;
                progressRecord.StatusDescription = $"Retrieved {info.Retrieved} of {total} users...";
                progressRecord.PercentComplete = percentComplete;
                WriteProgress(progressRecord);
            });

            await foreach (var user in _wizClient.GetUsersWithProgressAsyncEnumerable(PageSize, Type, ProjectId, MaxResults, progress, CancelToken)) {
                if (CancelToken.IsCancellationRequested)
                    break;

                if (Raw) {
                    WriteObject(user);
                } else {
                    var comprehensiveUser = WizUserComprehensive.FromWizUser(user);
                    WriteObject(comprehensiveUser);
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