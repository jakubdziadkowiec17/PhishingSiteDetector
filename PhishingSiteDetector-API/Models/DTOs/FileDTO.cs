using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class FileDTO
    {
        [Required]
        public IFormFile File { get; set; }
    }
}