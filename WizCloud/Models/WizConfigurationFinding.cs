using System;
using System.Collections.Generic;

namespace WizCloud;

/// <summary>
/// Represents a configuration finding from Wiz.
/// </summary>
public class WizConfigurationFinding {
    /// <summary>
    /// Gets or sets the unique identifier of the finding.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the finding.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the finding.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the severity of the finding.
    /// </summary>
    public WizSeverity? Severity { get; set; }

    /// <summary>
    /// Gets or sets the compliance frameworks associated with the finding.
    /// </summary>
    public List<string> ComplianceFrameworks { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets information about failed resources.
    /// </summary>
    public WizFailedResources? FailedResources { get; set; }

    /// <summary>
    /// Gets or sets the rule associated with the finding.
    /// </summary>
    public WizConfigRule? Rule { get; set; }

    /// <summary>
    /// Gets or sets remediation guidance for the finding.
    /// </summary>
    public string? Remediation { get; set; }

}

/// <summary>
/// Represents failed resource details for a configuration finding.
/// </summary>
public class WizFailedResources {
    /// <summary>Gets or sets the count of failed resources.</summary>
    public int Count { get; set; }

    /// <summary>Gets or sets the list of failed resources.</summary>
    public List<WizConfigurationFailedResource> Resources { get; set; } = new List<WizConfigurationFailedResource>();
}

/// <summary>
/// Represents a resource that failed a configuration check.
/// </summary>
public class WizConfigurationFailedResource {
    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource type.
    /// </summary>
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Represents the rule associated with a configuration finding.
/// </summary>
public class WizConfigRule {
    /// <summary>
    /// Gets or sets the rule identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rule name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rule category.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}
