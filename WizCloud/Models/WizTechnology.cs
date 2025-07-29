using System.Collections.Generic;

namespace WizCloud;
/// <summary>
/// Represents technology information associated with a user or resource in Wiz.
/// </summary>
public class WizTechnology {
    /// <summary>
    /// Gets or sets the unique identifier of the technology.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon identifier or URL for the technology.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the name of the technology.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the technology.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of categories associated with this technology.
    /// </summary>
    public List<WizCategory> Categories { get; set; } = new List<WizCategory>();
}