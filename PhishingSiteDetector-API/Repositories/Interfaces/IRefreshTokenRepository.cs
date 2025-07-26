using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task DeleteRefreshTokenAsync(RefreshToken refreshToken);
    }
}