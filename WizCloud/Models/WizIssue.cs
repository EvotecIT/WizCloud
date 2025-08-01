using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

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

    /// <summary>
    /// Creates a <see cref="WizIssue"/> from JSON.
    /// </summary>
    public static WizIssue FromJson(JsonNode node) {
        var issue = new WizIssue {
            Id = node["id"]?.GetValue<string>() ?? string.Empty,
            Name = node["name"]?.GetValue<string>() ?? string.Empty,
            Type = node["type"]?.GetValue<string>() ?? string.Empty,
            Severity = Enum.TryParse(node["severity"]?.GetValue<string>(), true, out WizSeverity tmpSev) ? tmpSev : null,
            Status = node["status"]?.GetValue<string>(),
            CreatedAt = node["createdAt"]?.GetValue<DateTime?>()?.ToLocalTime(),
            UpdatedAt = node["updatedAt"]?.GetValue<DateTime?>()?.ToLocalTime(),
            ResolvedAt = node["resolvedAt"]?.GetValue<DateTime?>()?.ToLocalTime(),
            DueAt = node["dueAt"]?.GetValue<DateTime?>()?.ToLocalTime()
        };

        var projects = node["projects"]?.AsArray();
        if (projects != null) {
            foreach (var proj in projects) {
                if (proj is JsonObject obj) {
                    issue.Projects.Add(new WizProject {
                        Id = obj["id"]?.GetValue<string>() ?? string.Empty,
                        Name = obj["name"]?.GetValue<string>() ?? string.Empty,
                        Slug = string.Empty,
                        IsFolder = false
                    });
                }
            }
        }

        var resource = node["resource"];
        if (resource != null) {
            issue.Resource = WizIssueResource.FromJson(resource);
        }

        var control = node["control"];
        if (control != null) {
            issue.Control = WizIssueControl.FromJson(control);
        }

        issue.Evidence = node["evidence"]?.GetValue<string>();
        issue.Remediation = node["remediation"]?.GetValue<string>();

        return issue;
    }
}
