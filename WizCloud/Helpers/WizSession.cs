namespace WizCloud;
/// <summary>
/// Provides session level defaults for Wiz operations.
/// </summary>
public static class WizSession {
    private static string? _defaultToken;
    private static string? _defaultClientId;
    private static string? _defaultClientSecret;
    private static WizRegion _defaultRegion = WizRegion.EU17;

    /// <summary>
    /// Gets or sets the default Wiz token for the session.
    /// </summary>
    public static string? DefaultToken {
        get => _defaultToken;
        set => _defaultToken = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz client id for the session.
    /// </summary>
    public static string? DefaultClientId {
        get => _defaultClientId;
        set => _defaultClientId = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz client secret for the session.
    /// </summary>
    public static string? DefaultClientSecret {
        get => _defaultClientSecret;
        set => _defaultClientSecret = value;
    }

    /// <summary>
    /// Gets or sets the default Wiz region for the session.
    /// </summary>
    public static WizRegion DefaultRegion {
        get => _defaultRegion;
        set => _defaultRegion = value;
    }
}