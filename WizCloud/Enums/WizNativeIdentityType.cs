namespace WizCloud;
/// <summary>
/// Represents provider-specific native identity types.
/// </summary>
public enum WizNativeIdentityType {
    /// <summary>Azure Active Directory user.</summary>
    AAD_USER,
    /// <summary>Azure Active Directory service principal.</summary>
    AAD_SERVICE_PRINCIPAL,
    /// <summary>AWS IAM user.</summary>
    AWS_IAM_USER,
    /// <summary>AWS IAM role.</summary>
    AWS_IAM_ROLE,
    /// <summary>Other or unknown native type.</summary>
    OTHER
}
