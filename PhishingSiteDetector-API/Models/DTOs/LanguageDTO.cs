using Microsoft.ML.Data;
using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class LanguageDTO
    {
        [Required]
        public string LanguageCode { get; set; }
    }
}