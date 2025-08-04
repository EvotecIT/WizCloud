using System;
using System.Collections.Generic;

namespace WizCloud;

/// <summary>
/// Represents a security issue returned by the Wiz API.
/// </summary>
public class WizIssue {
    /// <summary>
    /// Gets or sets the unique identifier of the issue.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the issue.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the issue.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity of the issue.
    /// </summary>
    public WizSeverity? Severity { get; set; }

    /// <summary>
    /// Gets or sets the status of the issue.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the resolution timestamp.
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Gets or sets the due date for the issue.
    /// </summary>
    public DateTime? DueAt { get; set; }

    /// <summary>
    /// Gets or sets the projects associated with this issue.
    /// </summary>
    public List<WizProject> Projects { get; set; } = new List<WizProject>();

    /// <summary>
    /// Gets or sets the affected resource.
    /// </summary>
    public WizIssueResource? Resource { get; set; }

    /// <summary>
    /// Gets or sets the control that triggered the issue.
    /// </summary>
    public WizIssueControl? Control { get; set; }

    /// <summary>
    /// Gets or sets evidence associated with the issue.
    /// </summary>
    public string? Evidence { get; set; }

    /// <summary>
    /// Gets or sets remediation guidance for the issue.
    /// </summary>
    public string? Remediation { get; set; }

}
