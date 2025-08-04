using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace WizCloud;

/// <summary>
/// Represents a comprehensive user object with all possible properties from Wiz API.
/// </summary>
public partial class WizUserComprehensive : WizUser {
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
            GraphEntity = user.GraphEntity,
            HasAccessToSensitiveData = user.HasAccessToSensitiveData,
            HasAdminPrivileges = user.HasAdminPrivileges,
            HasHighPrivileges = user.HasHighPrivileges,
            HasSensitiveData = user.HasSensitiveData,
            Projects = user.Projects,
            Technology = user.Technology,
            CloudAccount = user.CloudAccount,
            IssueAnalytics = user.IssueAnalytics
        };

        // Extract all properties from GraphEntity
        if (user.GraphEntity?.Properties != null) {
            var props = user.GraphEntity.Properties;
            // User identification
            comprehensive.UserPrincipalName = GetStringValue(props, "userPrincipalName");
            comprehensive.DisplayName = GetStringValue(props, "displayName");
            comprehensive.GivenName = GetStringValue(props, "givenName");
            comprehensive.Surname = GetStringValue(props, "surname");
            comprehensive.Email = GetStringValue(props, "email");
            comprehensive.Mail = GetStringValue(props, "mail");
            comprehensive.MailNickname = GetStringValue(props, "mailNickname");

            // Organization
            comprehensive.Company = GetStringValue(props, "company");
            comprehensive.Department = GetStringValue(props, "department");
            comprehensive.JobTitle = GetStringValue(props, "jobTitle");
            comprehensive.Location = GetStringValue(props, "location");
            comprehensive.Description = GetStringValue(props, "description");

            // Account status
            comprehensive.AccountEnabled = GetBoolValue(props, "accountEnabled");
            comprehensive.Active = GetBoolValue(props, "active");
            comprehensive.Enabled = GetBoolValue(props, "enabled");
            comprehensive.Status = GetStringValue(props, "status");
            comprehensive.UserType = GetStringValue(props, "userType");
            comprehensive.HasMfa = GetBoolValue(props, "hasMfa");
            comprehensive.InactiveInLast90Days = GetBoolValue(props, "inactiveInLast90Days");
            comprehensive.InactiveTimeframe = GetStringValue(props, "inactiveTimeframe");

            // Directory
            comprehensive.UserDirectory = GetStringValue(props, "userDirectory");
            comprehensive.PremisesDistinguishedName = GetStringValue(props, "premisesDistinguishedName");
            comprehensive.AadOnPremisesDomainName = GetStringValue(props, "aadOnPremisesDomainName");
            comprehensive.AadOnPremisesSamAccountName = GetStringValue(props, "aadOnPremisesSamAccountName");
            comprehensive.HomeDirectory = GetStringValue(props, "homeDirectory");
            comprehensive.ShellPath = GetStringValue(props, "shellPath");

            // Credentials
            comprehensive.CredentialId = GetStringValue(props, "credentialId");
            comprehensive.CredentialType = GetStringValue(props, "credentialType");
            comprehensive.EverUsed = GetBoolValue(props, "everUsed");
            comprehensive.ValidAfter = GetDateTimeValue(props, "validAfter");
            comprehensive.ValidBefore = GetDateTimeValue(props, "validBefore");
            comprehensive.RotatedAt = GetDateTimeValue(props, "rotatedAt");
            comprehensive.LastPasswordChange = GetDateTimeValue(props, "lastPasswordChange");

            // Service account
            comprehensive.ClientId = GetStringValue(props, "clientId");
            comprehensive.AadAppId = GetStringValue(props, "aad_appId");
            comprehensive.AadAppOwnerTenantId = GetStringValue(props, "aad_appOwnerTenantId");
            comprehensive.AadObjectId = GetStringValue(props, "aad_objectId");
            comprehensive.AadPublisherName = GetStringValue(props, "aad_publisherName");
            comprehensive.AadSignInAudience = GetStringValue(props, "aad_signInAudience");
            comprehensive.Managed = GetBoolValue(props, "managed");

            // Kubernetes
            comprehensive.KubernetesClusterExternalId = GetStringValue(props, "kubernetes_clusterExternalId");
            comprehensive.KubernetesClusterName = GetStringValue(props, "kubernetes_clusterName");
            comprehensive.KubernetesFlavor = GetStringValue(props, "kubernetes_kubernetesFlavor");
            comprehensive.Namespace = GetStringValue(props, "namespace");

            // Resources
            comprehensive.ExternalId = GetStringValue(props, "externalId");
            comprehensive.ProviderUniqueId = GetStringValue(props, "providerUniqueId");
            comprehensive.FullResourceName = GetStringValue(props, "fullResourceName");
            comprehensive.CloudProviderUrl = GetStringValue(props, "cloudProviderURL");
            comprehensive.Region = GetStringValue(props, "region");
            comprehensive.SubscriptionExternalId = GetStringValue(props, "subscriptionExternalId");

            // Timestamps
            comprehensive.CreatedAt = GetDateTimeValue(props, "createdAt");
            comprehensive.UpdatedAt = GetDateTimeValue(props, "updatedAt");
            comprehensive.DirectoryLastSyncTime = GetDateTimeValue(props, "directoryLastSyncTime");

            // Special properties
            comprehensive.VertexId = GetStringValue(props, "_vertexID");

            // Handle arrays
            comprehensive.OtherMails = GetStringListValue(props, "otherMails");
            comprehensive.ProxyAddresses = GetStringListValue(props, "proxyAddresses");
            comprehensive.ProductIds = GetStringListValue(props, "_productIDs");

            // Parse proxy addresses to extract email addresses
            ParseProxyAddresses(comprehensive);
        }

        // Extract project names for easy access
        if (comprehensive.Projects != null && comprehensive.Projects.Count > 0) {
            comprehensive.ProjectNames = comprehensive.Projects.Select(p => p.Name).ToList();
        }


        return comprehensive;
    }

    private static string? GetStringValue(Dictionary<string, JsonElement> properties, string key) {
        if (properties.TryGetValue(key, out var element)) {
            if (element.ValueKind == JsonValueKind.String)
                return element.GetString();
            return element.ToString();
        }
        return null;
    }

    private static bool? GetBoolValue(Dictionary<string, JsonElement> properties, string key) {
        if (properties.TryGetValue(key, out var element)) {
            if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
                return element.GetBoolean();
            if (element.ValueKind == JsonValueKind.String && bool.TryParse(element.GetString(), out var boolValue))
                return boolValue;
        }
        return null;
    }

    private static DateTime? GetDateTimeValue(Dictionary<string, JsonElement> properties, string key) {
        var stringValue = GetStringValue(properties, key);
        if (!string.IsNullOrEmpty(stringValue) && DateTime.TryParse(stringValue, out var dateValue)) {
            return dateValue;
        }
        return null;
    }

    private static List<string> GetStringListValue(Dictionary<string, JsonElement> properties, string key) {
        var list = new List<string>();
        if (properties.TryGetValue(key, out var element)) {
            if (element.ValueKind == JsonValueKind.Array) {
                foreach (var item in element.EnumerateArray()) {
                    var str = item.GetString();
                    if (!string.IsNullOrEmpty(str))
                        list.Add(str);
                }
            } else if (element.ValueKind == JsonValueKind.String) {
                var str = element.GetString();
                if (!string.IsNullOrEmpty(str))
                    list.Add(str);
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