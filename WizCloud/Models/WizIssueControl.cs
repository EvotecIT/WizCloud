using System;

namespace WizCloud;

/// <summary>
/// Represents the security control that generated an issue.
/// </summary>
public class WizIssueControl {
    /// <summary>
    /// Gets or sets the unique identifier of the control.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the control.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the severity of the control.
    /// </summary>
    public WizSeverity? Severity { get; set; }

}
