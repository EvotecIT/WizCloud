namespace WizCloud;
/// <summary>
/// Represents the native identity type in the cloud provider.
/// </summary>
public enum WizNativeType {
    /// <summary>
    /// Azure Active Directory user account.
    /// </summary>
    AADUser,

    /// <summary>
    /// Azure Active Directory group.
    /// </summary>
    AADGroup,

    /// <summary>
    /// AWS IAM user.
    /// </summary>
    AWSIamUser,

    /// <summary>
    /// Other or unknown type.
    /// </summary>
    UNKNOWN
}
