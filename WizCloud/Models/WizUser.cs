using System;
using System.Collections.Generic;
using System.Text.Json;

namespace WizCloud;

/// <summary>
/// Represents a user or identity principal in Wiz.
/// </summary>
public class WizUser {
    /// <summary>Gets or sets the unique identifier of the user.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the name of the user.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the type of the user.</summary>
    public WizUserType Type { get; set; } = WizUserType.USER_ACCOUNT;

    /// <summary>Gets or sets the native type in the cloud provider.</summary>
    public WizNativeType? NativeType { get; set; }

    /// <summary>Gets or sets the date and time when the user was deleted, if applicable.</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>Gets or sets graph entity information for the user.</summary>
    public WizUserGraphEntity? GraphEntity { get; set; }

    /// <summary>Gets or sets a value indicating whether the user has access to sensitive data.</summary>
    public bool HasAccessToSensitiveData { get; set; }

    /// <summary>Gets or sets a value indicating whether the user has administrator privileges.</summary>
    public bool HasAdminPrivileges { get; set; }

    /// <summary>Gets or sets a value indicating whether the user has high privileges.</summary>
    public bool HasHighPrivileges { get; set; }

    /// <summary>Gets or sets a value indicating whether the user has sensitive data.</summary>
    public bool HasSensitiveData { get; set; }

    /// <summary>Gets or sets the list of projects associated with this user.</summary>
    public List<WizProject> Projects { get; set; } = new List<WizProject>();

    /// <summary>Gets or sets the technology information associated with this user.</summary>
    public WizTechnology? Technology { get; set; }

    /// <summary>Gets or sets the cloud account information for this user.</summary>
    public WizCloudAccount? CloudAccount { get; set; }

    /// <summary>Gets or sets the issue analytics for this user.</summary>
    public WizIssueAnalytics? IssueAnalytics { get; set; }
}

/// <summary>
/// Represents the graph entity associated with a Wiz user.
/// </summary>
public class WizUserGraphEntity {
    /// <summary>Gets or sets the graph entity identifier.</summary>
    public string? Id { get; set; }

    /// <summary>Gets or sets the graph entity type.</summary>
    public WizGraphEntityType? Type { get; set; }

    /// <summary>Gets or sets additional graph entity properties.</summary>
    public Dictionary<string, JsonElement> Properties { get; set; } = new Dictionary<string, JsonElement>();
}
