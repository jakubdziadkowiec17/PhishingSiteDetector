namespace PhishingSiteDetector_API.Models.DTOs
{
    public class UrlFeaturesDTO
    {
        public float NumDots { get; set; }
        public float SubdomainLevel { get; set; }
        public float PathLevel { get; set; }
        public float UrlLength { get; set; }
        public float NumDash { get; set; }
        public float NumDashInHostname { get; set; }
        public float AtSymbol { get; set; }
        public float TildeSymbol { get; set; }
        public float NumUnderscore { get; set; }
        public float NumPercent { get; set; }
        public float NumQueryComponents { get; set; }
        public float NumAmpersand { get; set; }
        public float NumHash { get; set; }
        public float NumNumericChars { get; set; }
        public float NoHttps { get; set; }
        public float IpAddress { get; set; }
        public float HttpsInHostname { get; set; }
        public float HostnameLength { get; set; }
        public float PathLength { get; set; }
        public float QueryLength { get; set; }
        public float DoubleSlashInPath { get; set; }
    }
}