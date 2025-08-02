using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

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
    /// Gets or sets the count of failed resources.
    /// </summary>
    public int FailedResourceCount { get; set; }

    /// <summary>
    /// Gets or sets the list of failed resources.
    /// </summary>
    public List<WizConfigurationFailedResource> FailedResources { get; set; } = new List<WizConfigurationFailedResource>();

    /// <summary>
    /// Gets or sets the rule associated with the finding.
    /// </summary>
    public WizConfigRule? Rule { get; set; }

    /// <summary>
    /// Gets or sets remediation guidance for the finding.
    /// </summary>
    public string? Remediation { get; set; }

    /// <summary>
    /// Creates a <see cref="WizConfigurationFinding"/> from JSON.
    /// </summary>
    public static WizConfigurationFinding FromJson(JsonNode node) {
        var finding = new WizConfigurationFinding {
            Id = node["id"]?.GetValue<string>() ?? string.Empty,
            Title = node["title"]?.GetValue<string>() ?? string.Empty,
            Description = node["description"]?.GetValue<string>(),
            Severity = Enum.TryParse(node["severity"]?.GetValue<string>(), true, out WizSeverity tmpSev) ? tmpSev : null,
            Remediation = node["remediation"]?.GetValue<string>()
        };

        var frameworks = node["complianceFrameworks"]?.AsArray();
        if (frameworks != null) {
            foreach (var f in frameworks) {
                var val = f?.GetValue<string>();
                if (val != null)
                    finding.ComplianceFrameworks.Add(val);
            }
        }

        var failed = node["failedResources"];
        if (failed != null) {
            finding.FailedResourceCount = failed["count"]?.GetValue<int>() ?? 0;
            var resources = failed["resources"]?.AsArray();
            if (resources != null) {
                foreach (var res in resources) {
                    if (res != null) {
                        finding.FailedResources.Add(new WizConfigurationFailedResource {
                            Id = res["id"]?.GetValue<string>() ?? string.Empty,
                            Name = res["name"]?.GetValue<string>() ?? string.Empty,
                            Type = res["type"]?.GetValue<string>() ?? string.Empty
                        });
                    }
                }
            }
        }

        var rule = node["rule"];
        if (rule != null) {
            finding.Rule = new WizConfigRule {
                Id = rule["id"]?.GetValue<string>() ?? string.Empty,
                Name = rule["name"]?.GetValue<string>() ?? string.Empty,
                Category = rule["category"]?.GetValue<string>() ?? string.Empty
            };
        }

        return finding;
    }
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
