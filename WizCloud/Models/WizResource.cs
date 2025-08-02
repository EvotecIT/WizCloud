using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace WizCloud;
/// <summary>
/// Represents a generic cloud resource returned by the Wiz API.
/// </summary>
public class WizResource {
    /// <summary>Gets or sets the unique identifier of the resource.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the name of the resource.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the general type of the resource.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the native provider type of the resource.</summary>
    public string? NativeType { get; set; }

    /// <summary>Gets or sets the cloud platform hosting the resource.</summary>
    public WizCloudProvider? CloudPlatform { get; set; }

    /// <summary>Gets or sets the owning cloud account.</summary>
    public WizCloudAccount? CloudAccount { get; set; }

    /// <summary>Gets or sets the region where the resource resides.</summary>
    public string? Region { get; set; }

    /// <summary>Gets or sets the resource tags.</summary>
    public Dictionary<string, string> Tags { get; set; } = new();

    /// <summary>Gets or sets the creation timestamp.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Gets or sets the current status.</summary>
    public string? Status { get; set; }

    /// <summary>Gets or sets a value indicating whether the resource is publicly accessible.</summary>
    public bool PubliclyAccessible { get; set; }

    /// <summary>Gets or sets a value indicating whether the resource has a public IP address.</summary>
    public bool HasPublicIpAddress { get; set; }

    /// <summary>Gets or sets a value indicating whether the resource is internet facing.</summary>
    public bool IsInternetFacing { get; set; }

    /// <summary>Gets or sets the list of security groups associated with the resource.</summary>
    public List<string> SecurityGroups { get; set; } = new();

    /// <summary>Gets or sets issue counts for the resource.</summary>
    public WizResourceIssueCounts? Issues { get; set; }

    /// <summary>Creates a <see cref="WizResource"/> from JSON.</summary>
    public static WizResource FromJson(JsonNode node) {
        var resource = new WizResource {
            Id = node["id"]?.GetValue<string>() ?? string.Empty,
            Name = node["name"]?.GetValue<string>() ?? string.Empty,
            Type = node["type"]?.GetValue<string>() ?? string.Empty,
            NativeType = node["nativeType"]?.GetValue<string>(),
            CloudPlatform = Enum.TryParse(node["cloudPlatform"]?.GetValue<string>(), true, out WizCloudProvider tmpCp) ? tmpCp : null,
            Region = node["region"]?.GetValue<string>(),
            CreatedAt = node["createdAt"]?.GetValue<DateTime?>()?.ToLocalTime(),
            Status = node["status"]?.GetValue<string>(),
            PubliclyAccessible = node["publiclyAccessible"]?.GetValue<bool>() ?? false,
            HasPublicIpAddress = node["hasPublicIpAddress"]?.GetValue<bool>() ?? false,
            IsInternetFacing = node["isInternetFacing"]?.GetValue<bool>() ?? false
        };

        var tags = node["tags"] as JsonObject;
        if (tags != null) {
            foreach (var tag in tags) {
                resource.Tags[tag.Key] = tag.Value?.GetValue<string>() ?? string.Empty;
            }
        }

        var cloudAccount = node["cloudAccount"];
        if (cloudAccount != null) {
            resource.CloudAccount = WizCloudAccount.FromJson(cloudAccount);
        }

        var secGroups = node["securityGroups"]?.AsArray();
        if (secGroups != null) {
            foreach (var sg in secGroups) {
                if (sg != null) {
                    resource.SecurityGroups.Add(sg.GetValue<string>());
                }
            }
        }

        var issues = node["issues"];
        if (issues != null) {
            resource.Issues = WizResourceIssueCounts.FromJson(issues);
        }

        return resource;
    }
}
