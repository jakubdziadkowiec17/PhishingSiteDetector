using Microsoft.AspNetCore.Mvc;
using PhishingSiteDetector_API.Models.Domain;
using PhishingSiteDetector_API.Models.DTOs;

namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IDataSetService
    {
        Task<ResponseDTO> UploadAsync(DataSetDTO dataSetDTO);
        Task<ListPageDTO<DataSetItemDTO>> GetDataSetsAsync(string? searchText, int pageNumber, int pageSize, string sortField, int sortOrder);
        Task<FileData> DownloadDataSetAsync(int id);
        Task<ResponseDTO> UpdateActivityForDataSetAsync(int id, DataSetStatusDTO dataSetStatusDTO);
        Task<ResponseDTO> DeleteDataSetAsync(int id);
    }
}