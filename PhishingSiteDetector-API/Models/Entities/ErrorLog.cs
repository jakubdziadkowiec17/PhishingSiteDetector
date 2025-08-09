using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.Entities
{
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        public string? StackTrace { get; set; }
        public string? UserId { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
    }
}