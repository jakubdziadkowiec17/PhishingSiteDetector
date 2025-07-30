using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class UrlDTO
    {
        [Required]
        public string Url { get; set; }
        public bool IsRandomString { get; set; }
        public bool HasDomainInSubdomain { get; set; }
        public bool HasDomainInPath { get; set; }
        public bool HasEmbeddedBrandName { get; set; }
    }
}