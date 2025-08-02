using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Controllers
{
    [ApiController]
    [Route("api/site-log")]
    [Authorize(Roles = Role.Admin)]
    public class SiteLogController : ControllerBase
    {
        private readonly ISiteLogService _siteLogService;
        private readonly IErrorLogService _errorLogService;
        public SiteLogController(ISiteLogService siteLogService, IErrorLogService errorLogService)
        {
            _siteLogService = siteLogService;
            _errorLogService = errorLogService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChartDTO>>> GetSiteLogs(int year)
        {
            try
            {
                return Ok(await _siteLogService.GetSiteLogsAsync(year));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.DATA_SET_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_NOT_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.DELETING_DATA_SET_FAILED, ex));
                }
            }
        }
    }
}