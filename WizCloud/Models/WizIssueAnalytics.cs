using System.Collections.Generic;

namespace WizCloud;
/// <summary>
/// Represents analytics data for security issues associated with a user or resource.
/// </summary>
public class WizIssueAnalytics {
    /// <summary>
    /// Gets or sets the total count of all issues.
    /// </summary>
    public int IssueCount { get; set; }

    /// <summary>
    /// Gets or sets severity counts keyed by <see cref="WizSeverity"/>.
    /// </summary>
    public Dictionary<WizSeverity, int> SeverityCounts { get; } = new();
}