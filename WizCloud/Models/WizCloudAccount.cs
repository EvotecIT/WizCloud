using System.Text.Json.Nodes;

namespace WizCloud;

/// <summary>
/// Represents a cloud account in Wiz.
/// </summary>
public class WizCloudAccount {
    /// <summary>
    /// Gets or sets the unique identifier of the cloud account.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the cloud account.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cloud provider (e.g., AWS, Azure, GCP).
    /// </summary>
    public string CloudProvider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the external ID of the cloud account (e.g., AWS account ID, Azure subscription ID).
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Creates a WizCloudAccount from JSON.
    /// </summary>
    public static WizCloudAccount FromJson(JsonNode node) {
        return new WizCloudAccount {
            Id = node["id"]?.GetValue<string>() ?? string.Empty,
            Name = node["name"]?.GetValue<string>() ?? string.Empty,
            CloudProvider = node["cloudProvider"]?.GetValue<string>() ?? string.Empty,
            ExternalId = node["externalId"]?.GetValue<string>() ?? string.Empty
        };
    }
}