using System.Text.Json.Nodes;

namespace WizCloud;
/// <summary>
/// Represents issue summary counts for a resource.
/// </summary>
public class WizResourceIssueCounts {
    /// <summary>Gets or sets the critical issue count.</summary>
    public int CriticalCount { get; set; }
    /// <summary>Gets or sets the high issue count.</summary>
    public int HighCount { get; set; }
    /// <summary>Gets or sets the medium issue count.</summary>
    public int MediumCount { get; set; }
    /// <summary>Gets or sets the low issue count.</summary>
    public int LowCount { get; set; }

    /// <summary>Creates a <see cref="WizResourceIssueCounts"/> from JSON.</summary>
    public static WizResourceIssueCounts FromJson(JsonNode node) {
        return new WizResourceIssueCounts {
            CriticalCount = node["criticalCount"]?.GetValue<int>() ?? 0,
            HighCount = node["highCount"]?.GetValue<int>() ?? 0,
            MediumCount = node["mediumCount"]?.GetValue<int>() ?? 0,
            LowCount = node["lowCount"]?.GetValue<int>() ?? 0
        };
    }
}
