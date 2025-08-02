using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IErrorLogService _errorLogService;
        public AccountController(IAccountService accountService, IErrorLogService errorLogService)
        {
            _accountService = accountService;
            _errorLogService = errorLogService;
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                return Ok(await _accountService.LoginAsync(loginDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.INVALID_EMAIL_OR_PASSWORD:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.INVALID_EMAIL_OR_PASSWORD, ex));
                    case ERROR.USER_NOT_FOUND:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_FOUND, ex));
                    case ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE, ex));
                    default:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.LOGIN_FAILED, ex));
                }
            }
        }

        [HttpPost("refresh-tokens")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDTO>> RefreshTokens([FromBody] TokensForRefreshDTO tokensForRefreshDTO)
        {
            try
            {
                return Ok(await _accountService.RefreshTokensAsync(tokensForRefreshDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.YOUR_SESSION_HAS_EXPIRED:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.YOUR_SESSION_HAS_EXPIRED, ex));
                    case ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE, ex));
                    default:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.INVALID_CLIENT_REQUEST, ex));
                }
            }
        }

        [HttpGet("data")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<AccountDataDTO>> GetAccountData()
        {
            try
            {
                return Ok(await _accountService.GetAccountDataAsync());
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.GETTING_ACCOUNT_DATA_FAILED, ex));
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<AccountDTO>> GetAccount()
        {
            try
            {
                return Ok(await _accountService.GetAccountAsync());
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.GETTING_USER_DATA_FAILED, ex));
                }
            }
        }

        [HttpPut]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<ResponseDTO>> EditAccount([FromBody] AccountDTO accountDTO)
        {
            try
            {
                return Ok(await _accountService.EditAccountAsync(accountDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_FOUND, ex));
                    case ERROR.USER_WITH_THIS_EMAIL_ALREADY_EXISTS:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.USER_WITH_THIS_EMAIL_ALREADY_EXISTS, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.EDITING_ACCOUNT_FAILED, ex));
                }
            }
        }

        [HttpPut("change-language")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult> ChangeLanguage([FromBody] LanguageDTO languageDTO)
        {
            try
            {
                await _accountService.ChangeLanguageAsync(languageDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.CHANGING_LANGUAGE_FAILED, ex));
                }
            }
        }

        [HttpPut("reset-password")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<ResponseDTO>> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                return Ok(await _accountService.ResetPasswordAsync(resetPasswordDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_FOUND, ex));
                    case ERROR.OLD_PASSWORD_INCORRECT:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.OLD_PASSWORD_INCORRECT, ex));
                    case ERROR.NOT_THE_SAME_PASSWORD:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.NOT_THE_SAME_PASSWORD, ex));
                    case ERROR.INCORRECT_PASSWORD_RULES:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.INCORRECT_PASSWORD_RULES, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.PASSWORD_RESET_FAILED, ex));
                }
            }
        }

        [HttpPost("logout")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<ResponseDTO>> Logout([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            try
            {
                return Ok(await _accountService.LogoutAsync(refreshTokenDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.USER_NOT_RECOGNIZED:
                        return Unauthorized(await _errorLogService.CreateErrorLogAsync(ERROR.USER_NOT_RECOGNIZED, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.LOGOUT_FAILED, ex));
                }
            }
        }
    }
}