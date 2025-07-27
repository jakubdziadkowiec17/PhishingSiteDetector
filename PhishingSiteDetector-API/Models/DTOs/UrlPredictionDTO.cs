namespace PhishingSiteDetector_API.Models.DTOs
{
    public class UrlPredictionDTO
    {
        public bool IsPhishing { get; set; }
        public float Score { get; set; }
    }
}