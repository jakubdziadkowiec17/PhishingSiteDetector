using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class AccountDTO
    {
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
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
    }
}