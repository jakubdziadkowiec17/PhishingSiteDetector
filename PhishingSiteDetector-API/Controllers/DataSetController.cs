using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Controllers
{
    [ApiController]
    [Route("api/data-set")]
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
        public async Task<ActionResult<ResponseDTO>> UploadDataSet([FromForm] DataSetDTO dataSet)
        {
            try
            {
                return Ok(await _dataSetService.UploadAsync(dataSet));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.DATA_SET_IS_EMPTY:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_IS_EMPTY, ex));
                    case ERROR.DATA_SET_SHOULD_BE_CSV:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_SHOULD_BE_CSV, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_UPLOAD_FAILED, ex));
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<ListPageDTO<DataSetItemDTO>>> GetDataSets([FromQuery] string? searchText, [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string sortField, [FromQuery] int sortOrder)
        {
            try
            {
                return Ok(await _dataSetService.GetDataSetsAsync(searchText, pageNumber, pageSize, sortField, sortOrder));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.INVALID_SORT_FIELD_SPECIFIED:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.INVALID_SORT_FIELD_SPECIFIED, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.GETTING_DATA_SET_LIST_FAILED, ex));
                }
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
                switch (ex.Message)
                {
                    case ERROR.DATA_SET_NOT_FOUND:
                        return BadRequest(await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_NOT_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.DOWNLOADING_DATA_SET_FAILED, ex));
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDTO>> UpdateActivityForDataSet(int id, [FromBody] DataSetStatusDTO dataSetStatusDTO)
        {
            try
            {
                return Ok(await _dataSetService.UpdateActivityForDataSetAsync(id, dataSetStatusDTO));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case ERROR.DATA_SET_NOT_FOUND:
                        return NotFound(await _errorLogService.CreateErrorLogAsync(ERROR.DATA_SET_NOT_FOUND, ex));
                    default:
                        return StatusCode(500, await _errorLogService.CreateErrorLogAsync(ERROR.UPDATING_DATA_SET_ACTIVITY_FAILED, ex));
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDTO>> DeleteDataSet(int id)
        {
            try
            {
                return Ok(await _dataSetService.DeleteDataSetAsync(id));
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