namespace WizCloud;
/// <summary>
/// Represents a project in Wiz.
/// </summary>
public class WizProject {
    /// <summary>
    /// Gets or sets the unique identifier of the project.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the project.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug for the project.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this project is a folder.
    /// </summary>
    public bool IsFolder { get; set; }
}