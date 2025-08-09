using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Repositories.Interfaces
{
    public interface IDataSetRepository
    {
        Task<int> CreateDataSetAsync(DataSet dataSet);

        Task<IEnumerable<DataSet>> GetDataSetsAsync();

        Task<ListPageDTO<DataSet>> GetDataSetsAsync(string searchText, int pageNumber, int pageSize, string sortField, int sortOrder);

        Task<DataSet?> GetDataSetAsync(int id);

        Task<DataSet?> GetActiveDataSetAsync();

        Task UpdateActivityForDataSetAsync(DataSet dataSet);

        Task DeleteDataSetAsync(DataSet dataSet);
    }
}