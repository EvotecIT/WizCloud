using System;
using System.Text.Json.Nodes;

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

    /// <summary>
    /// Creates a <see cref="WizIssueControl"/> from JSON.
    /// </summary>
    public static WizIssueControl FromJson(JsonNode node) {
        return new WizIssueControl {
            Id = node["id"]?.GetValue<string>() ?? string.Empty,
            Name = node["name"]?.GetValue<string>() ?? string.Empty,
            Description = node["description"]?.GetValue<string>(),
            Severity = Enum.TryParse(node["severity"]?.GetValue<string>(), true, out WizSeverity tmpSev) ? tmpSev : null
        };
    }
}