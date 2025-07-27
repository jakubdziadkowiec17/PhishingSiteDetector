using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Controllers
{
    [ApiController]
    [Route("api/file")]
    [Authorize(Roles = Role.Admin)]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IErrorLogService _errorLogService;
        public FileController(IFileService fileService, IErrorLogService errorLogService)
        {
            _fileService = fileService;
            _errorLogService = errorLogService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> Upload([FromForm] FileDTO request)
        {
            try
            {
                return Ok(await _fileService.UploadAsync(request.File));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.FILE_IS_EMPTY:
                        return BadRequest(ERROR.FILE_IS_EMPTY);
                    case ERROR.FILE_SHOULD_BE_CSV:
                        return BadRequest(ERROR.FILE_SHOULD_BE_CSV);
                    default:
                        return StatusCode(500, ERROR.FILE_UPLOAD_FAILED);
                }
            }
        }
    }
}