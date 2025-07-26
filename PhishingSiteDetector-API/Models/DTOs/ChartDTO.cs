namespace PhishingSiteDetector_API.Models.DTOs
{
    public class ChartDTO
    {
        public string StartDate { get; set; }
        public int Value { get; set; }

        public ChartDTO(string startDate, int value)
        {
            StartDate = startDate;
            Value = value;
        }
    }
}