using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;
using PhishingSiteDetector_API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhishingSiteDetector_API.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AccountService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenRepository = refreshTokenRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<string> CreateAccessToken(ApplicationUser applicationUser)
        {
            string[] roles = [Role.Admin];
            var userRoles = await _userManager.GetRolesAsync(applicationUser);
            if (userRoles is null || userRoles.Count == 0)
            {
                throw new Exception(ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE);
            }

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, applicationUser.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:IssuerSigningKey"]));

            var accessToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:ValidityMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        public async Task<RefreshToken> CreateRefreshToken(ApplicationUser applicationUser)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = applicationUser.Id,
                ExpirationDate = DateTime.Now.AddDays(int.Parse(_configuration["RefreshToken:ValidityDays"]))
            };

            await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken);

            return refreshToken;
        }

        public async Task<TokensDTO> LoginAsync(LoginDTO loginDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, true, false);
            if (!result.Succeeded)
            {
                throw new Exception(ERROR.INVALID_EMAIL_OR_PASSWORD);
            }
            
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user is null)
            {
                throw new Exception(ERROR.USER_NOT_FOUND);
            }

            var accessToken = await CreateAccessToken(user);
            var refreshToken = await CreateRefreshToken(user);

            var tokens = new TokensDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

            return tokens;
        }

        public async Task<TokensDTO> RefreshTokensAsync(TokensForRefreshDTO tokensForRefreshDTO)
        {
            if (!string.IsNullOrEmpty(tokensForRefreshDTO.RefreshToken) && !string.IsNullOrEmpty(tokensForRefreshDTO.AccessToken))
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:IssuerSigningKey"])),
                    ValidateLifetime = false,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"]
                };

                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = jwtTokenHandler.ValidateToken(tokensForRefreshDTO.AccessToken, parameters, out var securityToken);

                if (securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (claimsPrincipal is not null)
                    {
                        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            var refreshTokenFromDB = await _refreshTokenRepository.GetRefreshTokenAsync(tokensForRefreshDTO.RefreshToken);
                            if (refreshTokenFromDB is not null && userId == refreshTokenFromDB.UserId)
                            {
                                await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshTokenFromDB);

                                if (refreshTokenFromDB.ExpirationDate < DateTime.Now)
                                {
                                    throw new Exception(ERROR.YOUR_SESSION_HAS_EXPIRED);
                                }

                                var user = await _userManager.FindByIdAsync(userId);
                                if (user is not null)
                                {
                                    var userRoles = await _userManager.GetRolesAsync(user);
                                    if (userRoles is null || userRoles.Count == 0)
                                    {
                                        throw new Exception(ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE);
                                    }
                                    
                                    var accessToken = await CreateAccessToken(user);
                                    var refreshToken = await CreateRefreshToken(user);
                                    
                                    var tokens = new TokensDTO
                                    {
                                        AccessToken = accessToken,
                                        RefreshToken = refreshToken.Token
                                    };

                                    return tokens;
                                }
                            }
                        }
                    }
                }
            }

            throw new Exception(ERROR.INVALID_CLIENT_REQUEST);
        }

        public async Task<AccountDataDTO> GetAccountDataAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser is null)
            {
                throw new Exception(ERROR.USER_NOT_FOUND);
            }

            return _mapper.Map<AccountDataDTO>(applicationUser);
        }

        public async Task<AccountDTO> GetAccountAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser is null)
            {
                throw new Exception(ERROR.USER_NOT_FOUND);
            }

            return _mapper.Map<AccountDTO>(applicationUser);
        }

        public async Task<ResponseDTO> EditAccountAsync(AccountDTO userDTO)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser is null)
            {
                throw new Exception(ERROR.USER_NOT_FOUND);
            }

            var userFromDb = await _userManager.FindByEmailAsync(userDTO.Email);
            if (userFromDb is not null && userFromDb.Email != applicationUser.Email)
            {
                throw new Exception(ERROR.USER_WITH_THIS_EMAIL_ALREADY_EXISTS);
            }

            _mapper.Map(userDTO, applicationUser);

            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new Exception(ERROR.EDITING_ACCOUNT_FAILED);
            }

            return new ResponseDTO(SUCCESS.ACCOUNT_EDITED);
        }

        public async Task ChangeLanguageAsync(LanguageDTO languageDTO)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser is null)
            {
                throw new Exception(ERROR.USER_NOT_FOUND);
            }

            _mapper.Map(languageDTO, applicationUser);

            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new Exception(ERROR.CHANGING_LANGUAGE_FAILED);
            }
        }

        public async Task<ResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser is null)
            {
                throw new Exception(ERROR.USER_NOT_FOUND);
            }

            var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(applicationUser, resetPasswordDTO.OldPassword);
            if (!isOldPasswordCorrect)
            {
                throw new Exception(ERROR.OLD_PASSWORD_INCORRECT);
            }

            if (resetPasswordDTO.NewPassword != resetPasswordDTO.ConfirmNewPassword)
            {
                throw new Exception(ERROR.NOT_THE_SAME_PASSWORD);
            }

            var regex = RegularExpression.Password;
            if (string.IsNullOrEmpty(resetPasswordDTO.NewPassword) || !regex.IsMatch(resetPasswordDTO.NewPassword))
            {
                throw new Exception(ERROR.INCORRECT_PASSWORD_RULES);
            }

            var passwordHash = _userManager.PasswordHasher.HashPassword(applicationUser, resetPasswordDTO.NewPassword);
            applicationUser.PasswordHash = passwordHash;
            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new Exception(ERROR.PASSWORD_RESET_FAILED);
            }

            return new ResponseDTO(SUCCESS.PASSWORD_RESET);
        }

        public async Task<ResponseDTO> LogoutAsync(RefreshTokenDTO refreshTokenDTO)
        {
            if (!string.IsNullOrEmpty(refreshTokenDTO.RefreshToken))
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;
                var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshTokenDTO.RefreshToken);

                if (refreshToken is not null)
                {
                    if (refreshToken.UserId == userId)
                    {
                        await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken);
                    }
                    else
                    {
                        throw new Exception(ERROR.USER_NOT_RECOGNIZED);
                    }
                }
            }

            await _signInManager.SignOutAsync();

            return new ResponseDTO(SUCCESS.LOGGED_OUT);
        }
    }
}