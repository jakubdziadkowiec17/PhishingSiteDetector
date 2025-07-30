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
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.INVALID_EMAIL_OR_PASSWORD:
                        return Unauthorized(ERROR.INVALID_EMAIL_OR_PASSWORD);
                    case ERROR.USER_NOT_FOUND:
                        return Unauthorized(ERROR.USER_NOT_FOUND);
                    case ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE:
                        return Unauthorized(ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE);
                    default:
                        return Unauthorized(ERROR.LOGIN_FAILED);
                }
            }

        }

        [HttpPost("refresh-tokens")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDTO>> RefreshTokens([FromBody] TokensDTO tokensDTO)
        {
            try
            {
                return Ok(await _accountService.RefreshTokensAsync(tokensDTO));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE:
                        return Unauthorized(ERROR.USER_NOT_ASSIGNED_TO_ANY_ROLE);
                    default:
                        return Unauthorized(ERROR.INVALID_CLIENT_REQUEST);
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
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(ERROR.USER_NOT_FOUND);
                    default:
                        return StatusCode(500, ERROR.GETTING_ACCOUNT_DATA_FAILED);
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
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(ERROR.USER_NOT_FOUND);
                    default:
                        return StatusCode(500, ERROR.GETTING_USER_DATA_FAILED);
                }
            }
        }

        [HttpPut]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<string>> EditAccount([FromBody] AccountDTO accountDTO)
        {
            try
            {
                return Ok(await _accountService.EditAccountAsync(accountDTO));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(ERROR.USER_NOT_FOUND);
                    case ERROR.USER_ALREADY_EXISTS:
                        return BadRequest(ERROR.USER_ALREADY_EXISTS);
                    default:
                        return StatusCode(500, ERROR.EDIT_ACCOUNT_FAILED);
                }
            }
        }

        [HttpPut("reset-password")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<string>> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                return Ok(await _accountService.ResetPasswordAsync(resetPasswordDTO));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_FOUND:
                        return NotFound(ERROR.USER_NOT_FOUND);
                    case ERROR.OLD_PASSWORD_INCORRECT:
                        return BadRequest(ERROR.OLD_PASSWORD_INCORRECT);
                    case ERROR.NOT_THE_SAME_PASSWORD:
                        return BadRequest(ERROR.NOT_THE_SAME_PASSWORD);
                    case ERROR.INCORRECT_PASSWORD_RULES:
                        return BadRequest(ERROR.INCORRECT_PASSWORD_RULES);
                    default:
                        return StatusCode(500, ERROR.PASSWORD_RESET_FAILED);
                }
            }
        }

        [HttpPost("logout")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<string>> Logout([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            try
            {
                return Ok(await _accountService.LogoutAsync(refreshTokenDTO));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_RECOGNIZED:
                        return Unauthorized(ERROR.USER_NOT_RECOGNIZED);
                    default:
                        return StatusCode(500, ERROR.LOGOUT_FAILED);
                }
            }
        }
    }
}