using PhishingSiteDetector_API.Models.Domain;
using PhishingSiteDetector_API.Models.DTOs;

namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IDataSetService
    {
        Task<string> UploadAsync(DataSetDTO dataSetDTO);
        Task<ListPageDTO<DataSetItemDTO>> GetDataSetsAsync(string? searchText, int pageNumber, int pageSize);
        Task<FileData> DownloadDataSetAsync(int id);
        Task<string> UpdateActivityForDataSetAsync(int id, DataSetStatusDTO dataSetStatusDTO);
        Task<string> DeleteDataSetAsync(int id);
    }
}