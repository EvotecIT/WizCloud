namespace WizCloud;
/// <summary>
/// Represents the different types of users and identities in Wiz.
/// </summary>
public enum WizUserType {
    /// <summary>
    /// A standard user account representing an individual user.
    /// </summary>
    USER_ACCOUNT,

    /// <summary>
    /// A service account used for automated processes and integrations.
    /// </summary>
    SERVICE_ACCOUNT,

    /// <summary>
    /// A group containing multiple users or accounts.
    /// </summary>
    GROUP,

    /// <summary>
    /// An access key used for programmatic access.
    /// </summary>
    ACCESS_KEY
}
