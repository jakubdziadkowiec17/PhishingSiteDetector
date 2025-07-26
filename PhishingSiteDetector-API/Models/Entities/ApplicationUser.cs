using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        [StringLength(100)]
        public string Address { get; set; }
        [Required]
        [Range(1, 9999)]
        public int AreaCode { get; set; }
        [Required]
        [Range(100000000, 999999999)]
        public int PhoneNumber { get; set; }
        [Required]
        public string LanguageCode { get; set; }
        public Language Language { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}