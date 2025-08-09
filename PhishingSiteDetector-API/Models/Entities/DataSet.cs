using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.Entities
{
    public class DataSet
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActiveDataSet { get; set; }
        [Required]
        public string AddedByUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
    }
}