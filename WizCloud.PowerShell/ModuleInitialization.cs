using System.Management.Automation;

namespace WizCloud.PowerShell;
/// <summary>
/// Module initialization and cleanup
/// </summary>
public class ModuleInitialization : IModuleAssemblyInitializer, IModuleAssemblyCleanup {
    /// <summary>
    /// Gets or sets the default Wiz token for the session
    /// </summary>
    public static string? DefaultToken {
        get => WizSession.DefaultToken;
        set => WizSession.DefaultToken = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz client id for the session
    /// </summary>
    public static string? DefaultClientId {
        get => WizSession.DefaultClientId;
        set => WizSession.DefaultClientId = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz client secret for the session
    /// </summary>
    public static string? DefaultClientSecret {
        get => WizSession.DefaultClientSecret;
        set => WizSession.DefaultClientSecret = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz region for the session
    /// </summary>
    public static WizRegion DefaultRegion {
        get => WizSession.DefaultRegion;
        set => WizSession.DefaultRegion = value;
    }

    /// <summary>
    /// Called when the module is imported
    /// </summary>
    public void OnImport() {
        // Nothing to do on import
    }

    /// <summary>
    /// Called when the module is removed
    /// </summary>
    public void OnRemove(PSModuleInfo psModuleInfo) {
        DefaultToken = null;
        DefaultClientId = null;
        DefaultClientSecret = null;
        DefaultRegion = WizRegion.EU17;
    }
}