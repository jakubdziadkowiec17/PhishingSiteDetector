using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.Entities
{
    public class Language
    {
        [Key]
        public string Code { get; set; }
        public ICollection<ApplicationUser>? ApplicationUsers { get; set; }
    }
}