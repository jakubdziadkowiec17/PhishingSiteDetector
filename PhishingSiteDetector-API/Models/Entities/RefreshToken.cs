using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
    }
}