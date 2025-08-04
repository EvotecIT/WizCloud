using System;
using System.Collections.Generic;

namespace WizCloud;

/// <summary>
/// Represents compliance posture information.
/// </summary>
public class WizComplianceResult {
    /// <summary>
    /// Gets or sets the framework name.
    /// </summary>
    public string Framework { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the overall compliance score.
    /// </summary>
    public double? OverallScore { get; set; }

    /// <summary>
    /// Gets the list of controls for this framework.
    /// </summary>
    public List<WizComplianceControl> Controls { get; set; } = new List<WizComplianceControl>();

    /// <summary>
    /// Gets or sets the date of the last assessment.
    /// </summary>
    public DateTime? LastAssessmentDate { get; set; }

}

/// <summary>
/// Represents a compliance control result.
/// </summary>
public class WizComplianceControl {
    /// <summary>
    /// Gets or sets the control identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the control name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the control status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the control severity.
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of failed resources.
    /// </summary>
    public int FailedResourceCount { get; set; }

    /// <summary>
    /// Gets or sets the number of passed resources.
    /// </summary>
    public int PassedResourceCount { get; set; }
}
