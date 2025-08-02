using Microsoft.ML.Data;

namespace PhishingSiteDetector_API.Models.Domain
{
    public class CsvColumns
    {
        [LoadColumn(1)] public float NumDots { get; set; }
        [LoadColumn(2)] public float SubdomainLevel { get; set; }
        [LoadColumn(3)] public float PathLevel { get; set; }
        [LoadColumn(4)] public float UrlLength { get; set; }
        [LoadColumn(5)] public float NumDash { get; set; }
        [LoadColumn(6)] public float NumDashInHostname { get; set; }
        [LoadColumn(7)] public float AtSymbol { get; set; }
        [LoadColumn(8)] public float TildeSymbol { get; set; }
        [LoadColumn(9)] public float NumUnderscore { get; set; }
        [LoadColumn(10)] public float NumPercent { get; set; }
        [LoadColumn(11)] public float NumQueryComponents { get; set; }
        [LoadColumn(12)] public float NumAmpersand { get; set; }
        [LoadColumn(13)] public float NumHash { get; set; }
        [LoadColumn(14)] public float NumNumericChars { get; set; }
        [LoadColumn(15)] public float NoHttps { get; set; }
        [LoadColumn(16)] public float RandomString { get; set; }
        [LoadColumn(17)] public float IpAddress { get; set; }
        [LoadColumn(18)] public float DomainInSubdomains { get; set; }
        [LoadColumn(19)] public float DomainInPaths { get; set; }
        [LoadColumn(20)] public float HttpsInHostname { get; set; }
        [LoadColumn(21)] public float HostnameLength { get; set; }
        [LoadColumn(22)] public float PathLength { get; set; }
        [LoadColumn(23)] public float QueryLength { get; set; }
        [LoadColumn(24)] public float DoubleSlashInPath { get; set; }
        [LoadColumn(26)] public float EmbeddedBrandName { get; set; }
        [LoadColumn(48)] public bool ClassLabel { get; set; }
    }
}