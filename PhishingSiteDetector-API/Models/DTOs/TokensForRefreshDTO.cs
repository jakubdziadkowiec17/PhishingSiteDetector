namespace PhishingSiteDetector_API.Models.DTOs
{
    public class TokensForRefreshDTO
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}