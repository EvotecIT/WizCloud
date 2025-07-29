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
    /// Gets or sets the count of informational severity issues.
    /// </summary>
    public int InformationalSeverityCount { get; set; }

    /// <summary>
    /// Gets or sets the count of low severity issues.
    /// </summary>
    public int LowSeverityCount { get; set; }

    /// <summary>
    /// Gets or sets the count of medium severity issues.
    /// </summary>
    public int MediumSeverityCount { get; set; }

    /// <summary>
    /// Gets or sets the count of high severity issues.
    /// </summary>
    public int HighSeverityCount { get; set; }

    /// <summary>
    /// Gets or sets the count of critical severity issues.
    /// </summary>
    public int CriticalSeverityCount { get; set; }
}