using System.Management.Automation;

namespace WizCloud.PowerShell;
/// <summary>
/// Module initialization and cleanup
/// </summary>
public class ModuleInitialization : IModuleAssemblyInitializer, IModuleAssemblyCleanup {
    private static string? _defaultToken = null;
    private static string? _defaultRegion = "eu17";

    /// <summary>
    /// Gets or sets the default Wiz token for the session
    /// </summary>
    public static string? DefaultToken {
        get => _defaultToken;
        set => _defaultToken = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz region for the session
    /// </summary>
    public static string? DefaultRegion {
        get => _defaultRegion;
        set => _defaultRegion = value;
    }

    /// <summary>
    /// Called when the module is imported
    /// </summary>
    public void OnImport() {
        // Check for environment variable on import
        var envToken = System.Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
        if (!string.IsNullOrEmpty(envToken)) {
            DefaultToken = envToken;
        }

        var envRegion = System.Environment.GetEnvironmentVariable("WIZ_REGION");
        if (!string.IsNullOrEmpty(envRegion)) {
            DefaultRegion = envRegion;
        }
    }

    /// <summary>
    /// Called when the module is removed
    /// </summary>
    public void OnRemove(PSModuleInfo psModuleInfo) {
        DefaultToken = null;
        DefaultRegion = "eu17";
    }
}