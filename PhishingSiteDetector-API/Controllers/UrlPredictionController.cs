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
                return Ok(await _urlPredictionService.PredictAsync(urlDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.NO_ACTIVE_DATA_SET_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.NO_ACTIVE_DATA_SET_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_PREDICTION_FAILED, ex));
                }
            }
        }
    }
}