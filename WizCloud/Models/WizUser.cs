using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace WizCloud;
/// <summary>
/// Represents a user or identity principal in Wiz.
/// </summary>
public class WizUser {
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the user (e.g., USER_ACCOUNT, SERVICE_ACCOUNT).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the native type in the cloud provider.
    /// </summary>
    public string? NativeType { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was deleted, if applicable.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Graph Entity Properties

    /// <summary>
    /// Gets or sets the graph entity identifier.
    /// </summary>
    public string? GraphEntityId { get; set; }

    /// <summary>
    /// Gets or sets the graph entity type.
    /// </summary>
    public string? GraphEntityType { get; set; }

    /// <summary>
    /// Gets or sets additional properties from the graph entity.
    /// </summary>
    public Dictionary<string, object?> GraphEntityProperties { get; set; } = new Dictionary<string, object?>();

    // Security Properties

    /// <summary>
    /// Gets or sets a value indicating whether the user has access to sensitive data.
    /// </summary>
    public bool HasAccessToSensitiveData { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has administrator privileges.
    /// </summary>
    public bool HasAdminPrivileges { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has high privileges.
    /// </summary>
    public bool HasHighPrivileges { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has sensitive data.
    /// </summary>
    public bool HasSensitiveData { get; set; }

    // Related Entities

    /// <summary>
    /// Gets or sets the list of projects associated with this user.
    /// </summary>
    public List<WizProject> Projects { get; set; } = new List<WizProject>();

    /// <summary>
    /// Gets or sets the technology information associated with this user.
    /// </summary>
    public WizTechnology? Technology { get; set; }

    /// <summary>
    /// Gets or sets the cloud account information for this user.
    /// </summary>
    public WizCloudAccount? CloudAccount { get; set; }

    /// <summary>
    /// Gets or sets the issue analytics for this user.
    /// </summary>
    public WizIssueAnalytics? IssueAnalytics { get; set; }

    /// <summary>
    /// Creates a WizUser instance from a JSON node.
    /// </summary>
    /// <param name="json">The JSON node containing user data.</param>
    /// <returns>A new WizUser instance populated with data from the JSON.</returns>
    public static WizUser FromJson(JsonNode json) {
        var user = new WizUser {
            Id = json["id"]?.GetValue<string>() ?? string.Empty,
            Name = json["name"]?.GetValue<string>() ?? string.Empty,
            Type = json["type"]?.GetValue<string>() ?? string.Empty,
            NativeType = json["nativeType"]?.GetValue<string>(),
            DeletedAt = json["deletedAt"]?.GetValue<DateTime?>()?.ToLocalTime(),

            HasAccessToSensitiveData = json["hasAccessToSensitiveData"]?.GetValue<bool>() ?? false,
            HasAdminPrivileges = json["hasAdminPrivileges"]?.GetValue<bool>() ?? false,
            HasHighPrivileges = json["hasHighPrivileges"]?.GetValue<bool>() ?? false,
            HasSensitiveData = json["hasSensitiveData"]?.GetValue<bool>() ?? false,

            Projects = new List<WizProject>(),
            GraphEntityProperties = new Dictionary<string, object?>()
        };

        // Parse graph entity
        var graphEntity = json["graphEntity"];
        if (graphEntity != null) {
            user.GraphEntityId = graphEntity["id"]?.GetValue<string>();
            user.GraphEntityType = graphEntity["type"]?.GetValue<string>();

            var properties = graphEntity["properties"];
            if (properties != null && properties is JsonObject propsObj) {
                foreach (var prop in propsObj) {
                    user.GraphEntityProperties[prop.Key] = prop.Value?.ToString();
                }
            }
        }

        // Parse projects
        var projects = json["projects"];
        if (projects != null && projects is JsonArray projectsArray) {
            foreach (var project in projectsArray) {
                if (project != null) {
                    user.Projects.Add(new WizProject {
                        Id = project["id"]?.GetValue<string>() ?? string.Empty,
                        Name = project["name"]?.GetValue<string>() ?? string.Empty,
                        Slug = project["slug"]?.GetValue<string>() ?? string.Empty,
                        IsFolder = project["isFolder"]?.GetValue<bool>() ?? false
                    });
                }
            }
        }

        // Parse technology
        var tech = json["technology"];
        if (tech != null) {
            user.Technology = new WizTechnology {
                Id = tech["id"]?.GetValue<string>() ?? string.Empty,
                Icon = tech["icon"]?.GetValue<string>(),
                Name = tech["name"]?.GetValue<string>() ?? string.Empty,
                Description = tech["description"]?.GetValue<string>(),
                Categories = new List<WizCategory>()
            };

            var categories = tech["categories"];
            if (categories != null && categories is JsonArray categoriesArray) {
                foreach (var cat in categoriesArray) {
                    if (cat != null) {
                        user.Technology.Categories.Add(new WizCategory {
                            Id = cat["id"]?.GetValue<string>() ?? string.Empty,
                            Name = cat["name"]?.GetValue<string>() ?? string.Empty
                        });
                    }
                }
            }
        }

        // Parse cloud account
        var cloudAccount = json["cloudAccount"];
        if (cloudAccount != null) {
            user.CloudAccount = new WizCloudAccount {
                Id = cloudAccount["id"]?.GetValue<string>() ?? string.Empty,
                Name = cloudAccount["name"]?.GetValue<string>() ?? string.Empty,
                CloudProvider = cloudAccount["cloudProvider"]?.GetValue<string>() ?? string.Empty,
                ExternalId = cloudAccount["externalId"]?.GetValue<string>()
            };
        }

        // Parse issue analytics
        var issueAnalytics = json["issueAnalytics"];
        if (issueAnalytics != null) {
            user.IssueAnalytics = new WizIssueAnalytics {
                IssueCount = issueAnalytics["issueCount"]?.GetValue<int>() ?? 0,
                InformationalSeverityCount = issueAnalytics["informationalSeverityCount"]?.GetValue<int>() ?? 0,
                LowSeverityCount = issueAnalytics["lowSeverityCount"]?.GetValue<int>() ?? 0,
                MediumSeverityCount = issueAnalytics["mediumSeverityCount"]?.GetValue<int>() ?? 0,
                HighSeverityCount = issueAnalytics["highSeverityCount"]?.GetValue<int>() ?? 0,
                CriticalSeverityCount = issueAnalytics["criticalSeverityCount"]?.GetValue<int>() ?? 0
            };
        }

        return user;
    }
}