using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Controllers
{
    [ApiController]
    [Route("api/url-prediction")]
    [AllowAnonymous]
    public class UrlPredictionController : ControllerBase
    {
        private readonly IUrlPredictionService _urlPredictionService;
        private readonly IErrorLogService _errorLogService;
        public UrlPredictionController(IUrlPredictionService urlPredictionService, IErrorLogService errorLogService)
        {
            _urlPredictionService = urlPredictionService;
            _errorLogService = errorLogService;
        }

        [HttpPost]
        public async Task<ActionResult<UrlPredictionDTO>> Predict([FromBody] UrlDTO urlDTO)
        {
            try
            {
                return Ok(_urlPredictionService.Predict(urlDTO));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.USER_NOT_RECOGNIZED:
                        return BadRequest(ERROR.USER_NOT_RECOGNIZED);
                    default:
                        return StatusCode(500, ERROR.LOGOUT_FAILED);
                }
            }
        }
    }
}