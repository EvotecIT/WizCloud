namespace WizCloud;
/// <summary>
/// Provides session level defaults for Wiz operations.
/// </summary>
public static class WizSession {
    private static string? _defaultToken;
    private static WizRegion _defaultRegion = WizRegion.EU17;

    /// <summary>
    /// Gets or sets the default Wiz token for the session.
    /// </summary>
    public static string? DefaultToken {
        get => _defaultToken;
        set => _defaultToken = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz region for the session.
    /// </summary>
    public static WizRegion DefaultRegion {
        get => _defaultRegion;
        set => _defaultRegion = value;
    }
}
