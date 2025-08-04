using System;
using System.Collections.Generic;

namespace WizCloud;

/// <summary>
/// Represents network exposure information.
/// </summary>
public class WizNetworkExposure {
    /// <summary>
    /// Gets or sets the exposure identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource associated with the exposure.
    /// </summary>
    public WizNetworkExposureResource? Resource { get; set; }

    /// <summary>
    /// Gets or sets the type of exposure.
    /// </summary>
    public string? ExposureType { get; set; }

    /// <summary>
    /// Gets the open ports for this exposure.
    /// </summary>
    public List<int> Ports { get; set; } = new List<int>();

    /// <summary>
    /// Gets the protocols for this exposure.
    /// </summary>
    public List<string> Protocols { get; set; } = new List<string>();

    /// <summary>
    /// Gets the source IP ranges for this exposure.
    /// </summary>
    public List<string> SourceIpRanges { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets whether the resource is internet facing.
    /// </summary>
    public bool? InternetFacing { get; set; }

    /// <summary>
    /// Gets or sets the public IP address.
    /// </summary>
    public string? PublicIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the DNS name.
    /// </summary>
    public string? DnsName { get; set; }

    /// <summary>
    /// Gets or sets the certificate information.
    /// </summary>
    public WizNetworkExposureCertificate? Certificate { get; set; }

}

/// <summary>
/// Represents a resource related to network exposure.
/// </summary>
public class WizNetworkExposureResource {
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
/// Represents certificate information for network exposure.
/// </summary>
public class WizNetworkExposureCertificate {
    /// <summary>
    /// Gets or sets the certificate issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the certificate expiry date.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the certificate is valid.
    /// </summary>
    public bool? IsValid { get; set; }
}
