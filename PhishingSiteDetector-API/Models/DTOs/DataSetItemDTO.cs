using System.ComponentModel.DataAnnotations;

namespace PhishingSiteDetector_API.Models.DTOs
{
    public class DataSetItemDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActiveDataSet { get; set; }
        [Required]
        public string CreationUserId { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
    }
}