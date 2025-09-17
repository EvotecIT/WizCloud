using System;
using System.Text.Json.Nodes;

namespace WizCloud;

/// <summary>
/// Represents the resource associated with a security issue.
/// </summary>
public class WizIssueResource {
    /// <summary>
    /// Gets or sets the unique identifier of the resource.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the resource.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the resource.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cloud platform hosting the resource.
    /// </summary>
    public WizCloudProvider? CloudPlatform { get; set; }

    /// <summary>
    /// Gets or sets the region of the resource.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the subscription identifier for the resource.
    /// </summary>
    public string? SubscriptionId { get; set; }

    /// <summary>
    /// Creates a <see cref="WizIssueResource"/> from JSON.
    /// </summary>
    public static WizIssueResource FromJson(JsonNode node) {
        return new WizIssueResource {
            Id = node["id"]?.GetValue<string>() ?? string.Empty,
            Name = node["name"]?.GetValue<string>() ?? string.Empty,
            Type = node["type"]?.GetValue<string>() ?? string.Empty,
            CloudPlatform = Enum.TryParse(node["cloudPlatform"]?.GetValue<string>(), true, out WizCloudProvider tmpCp) ? tmpCp : null,
            Region = node["region"]?.GetValue<string>(),
            SubscriptionId = node["subscriptionId"]?.GetValue<string>()
        };
    }
}