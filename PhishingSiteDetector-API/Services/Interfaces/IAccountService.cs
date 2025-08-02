using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using System.Security.Claims;

namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IAccountService
    {
        Task<string> CreateAccessToken(ApplicationUser applicationUser);
        Task<RefreshToken> CreateRefreshToken(ApplicationUser applicationUser);
        Task<TokensDTO> LoginAsync(LoginDTO model);
        Task<TokensDTO> RefreshTokensAsync(TokensForRefreshDTO tokensForRefreshDTO);
        Task<AccountDataDTO> GetAccountDataAsync();
        Task<AccountDTO> GetAccountAsync();
        Task<string> EditAccountAsync(AccountDTO userDTO);
        Task ChangeLanguageAsync(LanguageDTO languageDTO);
        Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<string> LogoutAsync(RefreshTokenDTO refreshTokenDTO);
    }
}