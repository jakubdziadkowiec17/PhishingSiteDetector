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
    public class DataSetController : ControllerBase
    {
        private readonly IDataSetService _dataSetService;
        private readonly IErrorLogService _errorLogService;
        public DataSetController(IDataSetService dataSetService, IErrorLogService errorLogService)
        {
            _dataSetService = dataSetService;
            _errorLogService = errorLogService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> Upload([FromForm] DataSetDTO dataSet)
        {
            try
            {
                return Ok(await _dataSetService.UploadAsync(dataSet));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.DATA_SET_IS_EMPTY:
                        return BadRequest(ERROR.DATA_SET_IS_EMPTY);
                    case ERROR.DATA_SET_SHOULD_BE_CSV:
                        return BadRequest(ERROR.DATA_SET_SHOULD_BE_CSV);
                    default:
                        return StatusCode(500, ERROR.DATA_SET_UPLOAD_FAILED);
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<ListPageDTO<DataSetItemDTO>>> GetDataSets([FromQuery] string? searchText, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                return Ok(await _dataSetService.GetDataSetsAsync(searchText, pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                return StatusCode(500, ERROR.GETTING_DATA_SET_LIST_FAILED);
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDataSet(int id)
        {
            try
            {
                var fileData = await _dataSetService.DownloadDataSetAsync(id);
                return File(fileData.Content, "text/csv", fileData.Name);
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.DATA_SET_NOT_FOUND:
                        return NotFound(ERROR.DATA_SET_NOT_FOUND);
                    default:
                        return StatusCode(500, ERROR.DOWNLOADING_DATA_SET_FAILED);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateActivityForDataSet(int id, [FromBody] DataSetItemDTO dataSetItemDTO)
        {
            try
            {
                return Ok(await _dataSetService.UpdateActivityForDataSetAsync(id, dataSetItemDTO));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.DATA_SET_NOT_FOUND:
                        return NotFound(ERROR.DATA_SET_NOT_FOUND);
                    default:
                        return StatusCode(500, ERROR.UPDATING_DATA_SET_ACTIVITY_FAILED);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteDataSet(int id)
        {
            try
            {
                return Ok(await _dataSetService.DeleteDataSetAsync(id));
            }
            catch (Exception ex)
            {
                await _errorLogService.CreateErrorLogAsync(ex);

                switch (ex.Message)
                {
                    case ERROR.DATA_SET_NOT_FOUND:
                        return NotFound(ERROR.DATA_SET_NOT_FOUND);
                    default:
                        return StatusCode(500, ERROR.DELETING_DATA_SET_FAILED);
                }
            }
        }
    }
}