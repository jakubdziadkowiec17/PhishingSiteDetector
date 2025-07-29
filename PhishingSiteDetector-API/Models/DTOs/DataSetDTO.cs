using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class DataSetDTO
    {
        [Required]
        public IFormFile File { get; set; }
    }
}