namespace WizCloud.Enums
{
    /// <summary>
    /// Represents the supported cloud providers in Wiz.
    /// </summary>
    public enum WizCloudProvider
    {
        /// <summary>
        /// Amazon Web Services (AWS).
        /// </summary>
        AWS,
        
        /// <summary>
        /// Microsoft Azure cloud platform.
        /// </summary>
        AZURE,
        
        /// <summary>
        /// Google Cloud Platform (GCP).
        /// </summary>
        GCP,
        
        /// <summary>
        /// Alibaba Cloud platform.
        /// </summary>
        ALIBABA,
        
        /// <summary>
        /// Oracle Cloud Infrastructure (OCI).
        /// </summary>
        OCI,
        
        /// <summary>
        /// Kubernetes clusters (any provider).
        /// </summary>
        KUBERNETES
    }
}