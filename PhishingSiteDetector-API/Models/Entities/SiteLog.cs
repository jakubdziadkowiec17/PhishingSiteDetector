using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.Entities
{
    public class SiteLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
    }
}