using System;


namespace WizCloud
{
    /// <summary>
    /// Provides helper methods for working with Wiz regions.
    /// </summary>
    public static class WizRegionHelper
    {
        /// <summary>
        /// Converts a WizRegion enum value to its corresponding API string representation.
        /// </summary>
        /// <param name="region">The region enum value to convert.</param>
        /// <returns>The lowercase string representation of the region for API calls.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid region value is provided.</exception>
        public static string ToApiString(WizRegion region)
        {
            switch (region)
            {
                case WizRegion.EU1:
                    return "eu1";
                case WizRegion.EU2:
                    return "eu2";
                case WizRegion.EU17:
                    return "eu17";
                case WizRegion.US1:
                    return "us1";
                case WizRegion.US2:
                    return "us2";
                case WizRegion.USGOV1:
                    return "usgov1";
                case WizRegion.AP1:
                    return "ap1";
                case WizRegion.AP2:
                    return "ap2";
                case WizRegion.CA1:
                    return "ca1";
                default:
                    throw new ArgumentOutOfRangeException(nameof(region), region, null);
            }
        }

        /// <summary>
        /// Converts a string representation of a region to its corresponding WizRegion enum value.
        /// </summary>
        /// <param name="region">The string representation of the region (case-insensitive).</param>
        /// <returns>The corresponding WizRegion enum value.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid region string is provided.</exception>
        public static WizRegion FromString(string region)
        {
            switch (region?.ToUpperInvariant())
            {
                case "EU1":
                    return WizRegion.EU1;
                case "EU2":
                    return WizRegion.EU2;
                case "EU17":
                    return WizRegion.EU17;
                case "US1":
                    return WizRegion.US1;
                case "US2":
                    return WizRegion.US2;
                case "USGOV1":
                    return WizRegion.USGOV1;
                case "AP1":
                    return WizRegion.AP1;
                case "AP2":
                    return WizRegion.AP2;
                case "CA1":
                    return WizRegion.CA1;
                default:
                    throw new ArgumentException($"Invalid region: {region}", nameof(region));
            }
        }
    }
}