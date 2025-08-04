using System;

namespace WizCloud;

/// <summary>
/// Represents an audit log entry returned by the Wiz API.
/// </summary>
public class WizAuditLogEntry {
    /// <summary>
    /// Gets or sets the unique identifier of the audit log entry.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp of the log entry.
    /// </summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the action.
    /// </summary>
    public WizAuditUser? User { get; set; }

    /// <summary>
    /// Gets or sets the action performed.
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Gets or sets the resource affected by the action.
    /// </summary>
    public WizAuditResource? Resource { get; set; }

    /// <summary>
    /// Gets or sets the status of the action.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the source IP address.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent string.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets additional details about the action.
    /// </summary>
    public string? Details { get; set; }

}

/// <summary>
/// Represents a user in an audit log entry.
/// </summary>
public class WizAuditUser {
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Represents a resource referenced in an audit log entry.
/// </summary>
public class WizAuditResource {
    /// <summary>
    /// Gets or sets the resource type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
