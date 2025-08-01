using System;
using System.Collections.Generic;
using System.Linq;

namespace WizCloud;

/// <summary>
/// Represents a comprehensive user object with all possible properties from Wiz API.
/// </summary>
public class WizUserComprehensive : WizUser {
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


    /// <summary>
    /// Creates a comprehensive WizUser from a regular WizUser by extracting all properties.
    /// </summary>
    public static WizUserComprehensive FromWizUser(WizUser user) {
        var comprehensive = new WizUserComprehensive {
            // Copy base properties
            Id = user.Id,
            Name = user.Name,
            Type = user.Type,
            NativeType = user.NativeType,
            DeletedAt = user.DeletedAt,
            GraphEntityId = user.GraphEntityId,
            GraphEntityType = user.GraphEntityType,
            GraphEntityProperties = user.GraphEntityProperties,
            HasAccessToSensitiveData = user.HasAccessToSensitiveData,
            HasAdminPrivileges = user.HasAdminPrivileges,
            HasHighPrivileges = user.HasHighPrivileges,
            HasSensitiveData = user.HasSensitiveData,
            Projects = user.Projects,
            Technology = user.Technology,
            CloudAccount = user.CloudAccount,
            IssueAnalytics = user.IssueAnalytics
        };

        // Extract all properties from GraphEntityProperties
        if (user.GraphEntityProperties != null) {
            // User identification
            comprehensive.UserPrincipalName = GetStringValue(user.GraphEntityProperties, "userPrincipalName");
            comprehensive.DisplayName = GetStringValue(user.GraphEntityProperties, "displayName");
            comprehensive.GivenName = GetStringValue(user.GraphEntityProperties, "givenName");
            comprehensive.Surname = GetStringValue(user.GraphEntityProperties, "surname");
            comprehensive.Email = GetStringValue(user.GraphEntityProperties, "email");
            comprehensive.Mail = GetStringValue(user.GraphEntityProperties, "mail");
            comprehensive.MailNickname = GetStringValue(user.GraphEntityProperties, "mailNickname");

            // Organization
            comprehensive.Company = GetStringValue(user.GraphEntityProperties, "company");
            comprehensive.Department = GetStringValue(user.GraphEntityProperties, "department");
            comprehensive.JobTitle = GetStringValue(user.GraphEntityProperties, "jobTitle");
            comprehensive.Location = GetStringValue(user.GraphEntityProperties, "location");
            comprehensive.Description = GetStringValue(user.GraphEntityProperties, "description");

            // Account status
            comprehensive.AccountEnabled = GetBoolValue(user.GraphEntityProperties, "accountEnabled");
            comprehensive.Active = GetBoolValue(user.GraphEntityProperties, "active");
            comprehensive.Enabled = GetBoolValue(user.GraphEntityProperties, "enabled");
            comprehensive.Status = GetStringValue(user.GraphEntityProperties, "status");
            comprehensive.UserType = GetStringValue(user.GraphEntityProperties, "userType");
            comprehensive.HasMfa = GetBoolValue(user.GraphEntityProperties, "hasMfa");
            comprehensive.InactiveInLast90Days = GetBoolValue(user.GraphEntityProperties, "inactiveInLast90Days");
            comprehensive.InactiveTimeframe = GetStringValue(user.GraphEntityProperties, "inactiveTimeframe");

            // Directory
            comprehensive.UserDirectory = GetStringValue(user.GraphEntityProperties, "userDirectory");
            comprehensive.PremisesDistinguishedName = GetStringValue(user.GraphEntityProperties, "premisesDistinguishedName");
            comprehensive.AadOnPremisesDomainName = GetStringValue(user.GraphEntityProperties, "aadOnPremisesDomainName");
            comprehensive.AadOnPremisesSamAccountName = GetStringValue(user.GraphEntityProperties, "aadOnPremisesSamAccountName");
            comprehensive.HomeDirectory = GetStringValue(user.GraphEntityProperties, "homeDirectory");
            comprehensive.ShellPath = GetStringValue(user.GraphEntityProperties, "shellPath");

            // Credentials
            comprehensive.CredentialId = GetStringValue(user.GraphEntityProperties, "credentialId");
            comprehensive.CredentialType = GetStringValue(user.GraphEntityProperties, "credentialType");
            comprehensive.EverUsed = GetBoolValue(user.GraphEntityProperties, "everUsed");
            comprehensive.ValidAfter = GetDateTimeValue(user.GraphEntityProperties, "validAfter");
            comprehensive.ValidBefore = GetDateTimeValue(user.GraphEntityProperties, "validBefore");
            comprehensive.RotatedAt = GetDateTimeValue(user.GraphEntityProperties, "rotatedAt");
            comprehensive.LastPasswordChange = GetDateTimeValue(user.GraphEntityProperties, "lastPasswordChange");

            // Service account
            comprehensive.ClientId = GetStringValue(user.GraphEntityProperties, "clientId");
            comprehensive.AadAppId = GetStringValue(user.GraphEntityProperties, "aad_appId");
            comprehensive.AadAppOwnerTenantId = GetStringValue(user.GraphEntityProperties, "aad_appOwnerTenantId");
            comprehensive.AadObjectId = GetStringValue(user.GraphEntityProperties, "aad_objectId");
            comprehensive.AadPublisherName = GetStringValue(user.GraphEntityProperties, "aad_publisherName");
            comprehensive.AadSignInAudience = GetStringValue(user.GraphEntityProperties, "aad_signInAudience");
            comprehensive.Managed = GetBoolValue(user.GraphEntityProperties, "managed");

            // Kubernetes
            comprehensive.KubernetesClusterExternalId = GetStringValue(user.GraphEntityProperties, "kubernetes_clusterExternalId");
            comprehensive.KubernetesClusterName = GetStringValue(user.GraphEntityProperties, "kubernetes_clusterName");
            comprehensive.KubernetesFlavor = GetStringValue(user.GraphEntityProperties, "kubernetes_kubernetesFlavor");
            comprehensive.Namespace = GetStringValue(user.GraphEntityProperties, "namespace");

            // Resources
            comprehensive.ExternalId = GetStringValue(user.GraphEntityProperties, "externalId");
            comprehensive.ProviderUniqueId = GetStringValue(user.GraphEntityProperties, "providerUniqueId");
            comprehensive.FullResourceName = GetStringValue(user.GraphEntityProperties, "fullResourceName");
            comprehensive.CloudProviderUrl = GetStringValue(user.GraphEntityProperties, "cloudProviderURL");
            comprehensive.Region = GetStringValue(user.GraphEntityProperties, "region");
            comprehensive.SubscriptionExternalId = GetStringValue(user.GraphEntityProperties, "subscriptionExternalId");

            // Timestamps
            comprehensive.CreatedAt = GetDateTimeValue(user.GraphEntityProperties, "createdAt");
            comprehensive.UpdatedAt = GetDateTimeValue(user.GraphEntityProperties, "updatedAt");
            comprehensive.DirectoryLastSyncTime = GetDateTimeValue(user.GraphEntityProperties, "directoryLastSyncTime");

            // Special properties
            comprehensive.VertexId = GetStringValue(user.GraphEntityProperties, "_vertexID");

            // Handle arrays
            comprehensive.OtherMails = GetStringListValue(user.GraphEntityProperties, "otherMails");
            comprehensive.ProxyAddresses = GetStringListValue(user.GraphEntityProperties, "proxyAddresses");
            comprehensive.ProductIds = GetStringListValue(user.GraphEntityProperties, "_productIDs");

            // Parse proxy addresses to extract email addresses
            ParseProxyAddresses(comprehensive);
        }

        // Extract project names for easy access
        if (comprehensive.Projects != null && comprehensive.Projects.Count > 0) {
            comprehensive.ProjectNames = comprehensive.Projects.Select(p => p.Name).ToList();
        }


        return comprehensive;
    }

    private static string? GetStringValue(Dictionary<string, object?> properties, string key) {
        if (properties.TryGetValue(key, out var value) && value != null) {
            return value.ToString();
        }
        return null;
    }

    private static bool? GetBoolValue(Dictionary<string, object?> properties, string key) {
        if (properties.TryGetValue(key, out var value) && value != null) {
            if (bool.TryParse(value.ToString(), out var boolValue)) {
                return boolValue;
            }
        }
        return null;
    }

    private static DateTime? GetDateTimeValue(Dictionary<string, object?> properties, string key) {
        var stringValue = GetStringValue(properties, key);
        if (!string.IsNullOrEmpty(stringValue) && DateTime.TryParse(stringValue, out var dateValue)) {
            return dateValue.ToLocalTime();
        }
        return null;
    }

    private static List<string> GetStringListValue(Dictionary<string, object?> properties, string key) {
        var list = new List<string>();
        if (properties.TryGetValue(key, out var value) && value != null) {
            if (value is List<object?> objectList) {
                foreach (var item in objectList) {
                    if (item != null) {
                        list.Add(item.ToString()!);
                    }
                }
            } else if (value is System.Text.Json.JsonElement jsonElement) {
                // Handle JsonElement arrays
                if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array) {
                    foreach (var item in jsonElement.EnumerateArray()) {
                        var str = item.GetString();
                        if (!string.IsNullOrEmpty(str)) {
                            list.Add(str!);
                        }
                    }
                } else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String) {
                    var str = jsonElement.GetString();
                    if (!string.IsNullOrEmpty(str)) {
                        list.Add(str!);
                    }
                }
            } else if (value is string stringValue && !string.IsNullOrEmpty(stringValue)) {
                // Single value as string
                list.Add(stringValue);
            }
        }
        return list;
    }

    private static void ParseProxyAddresses(WizUserComprehensive user) {
        if (user.ProxyAddresses == null || user.ProxyAddresses.Count == 0) {
            return;
        }

        foreach (var proxyAddress in user.ProxyAddresses) {
            if (string.IsNullOrWhiteSpace(proxyAddress)) {
                continue;
            }

            // Parse proxy address format (e.g., "smtp:email@domain.com", "SMTP:email@domain.com", "x500:/o=...")
            var parts = proxyAddress.Split(new[] { ':' }, 2);
            if (parts.Length == 2) {
                var prefix = parts[0].ToLower();
                var address = parts[1];

                if (prefix == "smtp") {
                    user.EmailAddresses.Add(address);
                    // SMTP in uppercase indicates primary address
                    if (parts[0] == "SMTP") {
                        user.PrimarySmtpAddress = address;
                    }
                }
                // You can add other types like SIP, X500, etc. if needed
            }
        }

        // Remove duplicates
        user.EmailAddresses = user.EmailAddresses.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }
}