using System.Management.Automation;

namespace WizCloud.PowerShell;

/// <summary>Clears Wiz authentication from the current session.</summary>
/// <para>Removes stored tokens, credentials, and region information for the session.</para>
/// <list type="alertSet">
/// <item>
/// <description>Disconnecting deletes authentication details and cannot be undone for the current session.</description>
/// </item>
/// </list>
/// <example>
/// <summary>Disconnect the current session</summary>
/// <code><prefix>PS&gt; </prefix>Disconnect-Wiz</code>
/// <para>This example clears any stored Wiz authentication information.</para>
/// </example>
/// <example>
/// <summary>Disconnect after connecting</summary>
/// <code><prefix>PS&gt; </prefix>Connect-Wiz -Token 'token'; Disconnect-Wiz</code>
/// <para>The connection created with <c>Connect-Wiz</c> is removed.</para>
/// </example>
/// <seealso href="https://learn.microsoft.com/powershell/scripting/overview">PowerShell documentation</seealso>
/// <seealso href="https://github.com/EvotecIT/WizCloud">Project documentation</seealso>
[Cmdlet(VerbsCommunications.Disconnect, "Wiz")]
[OutputType(typeof(bool))]
public sealed class CmdletDisconnectWiz : PSCmdlet {
    /// <inheritdoc/>
    protected override void ProcessRecord() {
        ModuleInitialization.DefaultToken = null;
        ModuleInitialization.DefaultClientId = null;
        ModuleInitialization.DefaultClientSecret = null;
        ModuleInitialization.DefaultRegion = WizRegion.EU17;
        WriteObject(true);
    }
}