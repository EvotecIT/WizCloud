using System.Management.Automation;

namespace WizCloud.PowerShell;

/// <summary>
/// <para type="synopsis">Clears Wiz authentication from the current session.</para>
/// <para type="description">The Disconnect-Wiz cmdlet removes stored authentication and client credentials from the session.</para>
/// <example>
/// <para>Disconnect from Wiz:</para>
/// <code>Disconnect-Wiz</code>
/// </example>
/// </summary>
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