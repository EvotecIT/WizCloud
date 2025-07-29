namespace WizCloud
{
    /// <summary>
    /// Represents the severity levels for issues in Wiz.
    /// </summary>
    public enum WizSeverity
    {
        /// <summary>
        /// Informational severity - No immediate action required.
        /// </summary>
        INFORMATIONAL,
        
        /// <summary>
        /// Low severity - Minor issues that should be addressed.
        /// </summary>
        LOW,
        
        /// <summary>
        /// Medium severity - Issues that require attention.
        /// </summary>
        MEDIUM,
        
        /// <summary>
        /// High severity - Important issues that should be prioritized.
        /// </summary>
        HIGH,
        
        /// <summary>
        /// Critical severity - Urgent issues requiring immediate attention.
        /// </summary>
        CRITICAL
    }
}