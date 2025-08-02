using System;
using System.Collections.Generic;

namespace WizCloud;

public partial class WizUserComprehensive {
    // Azure AD User Properties
    
    /// <summary>
    /// Gets or sets the user principal name (UPN).
    /// </summary>
    public string? UserPrincipalName { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the given name (first name).
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// Gets or sets the surname (last name).
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the mail address.
    /// </summary>
    public string? Mail { get; set; }

    /// <summary>
    /// Gets or sets the mail nickname.
    /// </summary>
    public string? MailNickname { get; set; }

    /// <summary>
    /// Gets or sets other email addresses.
    /// </summary>
    public List<string> OtherMails { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets proxy addresses.
    /// </summary>
    public List<string> ProxyAddresses { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets parsed email addresses from proxy addresses.
    /// </summary>
    public List<string> EmailAddresses { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the primary SMTP address.
    /// </summary>
    public string? PrimarySmtpAddress { get; set; }

    // Organization Properties

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the department.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the job title.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    // Account Status Properties

    /// <summary>
    /// Gets or sets whether the account is enabled.
    /// </summary>
    public bool? AccountEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether the user/credential is active.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Gets or sets whether the account is enabled (for service accounts).
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets the status (e.g., Active).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the user type (Member, Guest).
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// Gets or sets whether the user has MFA enabled.
    /// </summary>
    public bool? HasMfa { get; set; }

    /// <summary>
    /// Gets or sets whether the user was inactive in the last 90 days.
    /// </summary>
    public bool? InactiveInLast90Days { get; set; }

    /// <summary>
    /// Gets or sets the inactive timeframe.
    /// </summary>
    public string? InactiveTimeframe { get; set; }

    // Directory Properties

    /// <summary>
    /// Gets or sets the user directory (e.g., Azure, Kubernetes).
    /// </summary>
    public string? UserDirectory { get; set; }

    /// <summary>
    /// Gets or sets the on-premises distinguished name.
    /// </summary>
    public string? PremisesDistinguishedName { get; set; }

    /// <summary>
    /// Gets or sets the AAD on-premises domain name.
    /// </summary>
    public string? AadOnPremisesDomainName { get; set; }

    /// <summary>
    /// Gets or sets the AAD on-premises SAM account name.
    /// </summary>
    public string? AadOnPremisesSamAccountName { get; set; }

    /// <summary>
    /// Gets or sets the home directory.
    /// </summary>
    public string? HomeDirectory { get; set; }

    /// <summary>
    /// Gets or sets the shell path.
    /// </summary>
    public string? ShellPath { get; set; }

    // Credential Properties

    /// <summary>
    /// Gets or sets the credential ID.
    /// </summary>
    public string? CredentialId { get; set; }

    /// <summary>
    /// Gets or sets the credential type.
    /// </summary>
    public string? CredentialType { get; set; }

    /// <summary>
    /// Gets or sets whether the credential has ever been used.
    /// </summary>
    public bool? EverUsed { get; set; }

    /// <summary>
    /// Gets or sets when the credential is valid after.
    /// </summary>
    public DateTime? ValidAfter { get; set; }

    /// <summary>
    /// Gets or sets when the credential is valid before (expiration).
    /// </summary>
    public DateTime? ValidBefore { get; set; }

    /// <summary>
    /// Gets or sets when the credential was rotated.
    /// </summary>
    public DateTime? RotatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the password was last changed.
    /// </summary>
    public DateTime? LastPasswordChange { get; set; }

    // Service Account Properties

    /// <summary>
    /// Gets or sets the client ID (for service accounts).
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the AAD application ID.
    /// </summary>
    public string? AadAppId { get; set; }

    /// <summary>
    /// Gets or sets the AAD application owner tenant ID.
    /// </summary>
    public string? AadAppOwnerTenantId { get; set; }

    /// <summary>
    /// Gets or sets the AAD object ID.
    /// </summary>
    public string? AadObjectId { get; set; }

    /// <summary>
    /// Gets or sets the AAD publisher name.
    /// </summary>
    public string? AadPublisherName { get; set; }

    /// <summary>
    /// Gets or sets the AAD sign-in audience.
    /// </summary>
    public string? AadSignInAudience { get; set; }

    /// <summary>
    /// Gets or sets whether the service account is managed.
    /// </summary>
    public bool? Managed { get; set; }

    // Kubernetes Properties

    /// <summary>
    /// Gets or sets the Kubernetes cluster external ID.
    /// </summary>
    public string? KubernetesClusterExternalId { get; set; }

    /// <summary>
    /// Gets or sets the Kubernetes cluster name.
    /// </summary>
    public string? KubernetesClusterName { get; set; }

    /// <summary>
    /// Gets or sets the Kubernetes flavor (e.g., AKS).
    /// </summary>
    public string? KubernetesFlavor { get; set; }

    /// <summary>
    /// Gets or sets the namespace.
    /// </summary>
    public string? Namespace { get; set; }

    // Resource Properties

    /// <summary>
    /// Gets or sets the external ID.
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Gets or sets the provider unique ID.
    /// </summary>
    public string? ProviderUniqueId { get; set; }

    /// <summary>
    /// Gets or sets the full resource name.
    /// </summary>
    public string? FullResourceName { get; set; }

    /// <summary>
    /// Gets or sets the cloud provider URL.
    /// </summary>
    public string? CloudProviderUrl { get; set; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the subscription external ID.
    /// </summary>
    public string? SubscriptionExternalId { get; set; }

    // Timestamps

    /// <summary>
    /// Gets or sets when the entity was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the directory was last synced.
    /// </summary>
    public DateTime? DirectoryLastSyncTime { get; set; }

    // Product Association

    /// <summary>
    /// Gets or sets the list of product IDs associated with this user.
    /// </summary>
    public List<string> ProductIds { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the vertex ID.
    /// </summary>
    public string? VertexId { get; set; }

    /// <summary>
    /// Gets or sets the list of project names for easy access.
    /// </summary>
    public List<string> ProjectNames { get; set; } = new List<string>();
}
