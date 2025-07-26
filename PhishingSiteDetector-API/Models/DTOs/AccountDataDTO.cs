using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class AccountDataDTO
    {
        [Required]
        public string LanguageCode { get; set; }
    }
}