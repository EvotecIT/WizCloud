using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

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

    /// <summary>
    /// Creates a <see cref="WizComplianceResult"/> from JSON.
    /// </summary>
    public static WizComplianceResult FromJson(JsonNode node) {
        var result = new WizComplianceResult {
            Framework = node["framework"]?.GetValue<string>() ?? string.Empty,
            OverallScore = node["overallScore"]?.GetValue<double?>(),
            LastAssessmentDate = node["lastAssessmentDate"]?.GetValue<DateTime?>()?.ToLocalTime()
        };

        var controls = node["controls"]?.AsArray();
        if (controls != null) {
            foreach (var control in controls) {
                if (control != null) {
                    result.Controls.Add(new WizComplianceControl {
                        Id = control["id"]?.GetValue<string>() ?? string.Empty,
                        Name = control["name"]?.GetValue<string>() ?? string.Empty,
                        Status = control["status"]?.GetValue<string>() ?? string.Empty,
                        Severity = control["severity"]?.GetValue<string>() ?? string.Empty,
                        FailedResourceCount = control["failedResourceCount"]?.GetValue<int>() ?? 0,
                        PassedResourceCount = control["passedResourceCount"]?.GetValue<int>() ?? 0
                    });
                }
            }
        }

        return result;
    }
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
