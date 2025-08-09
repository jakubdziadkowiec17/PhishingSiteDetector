using Microsoft.EntityFrameworkCore;
using PhishingSiteDetector_API.Constants;
using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;

namespace PhishingSiteDetector_API.Repositories.Implementations
{
    public class DataSetRepository : IDataSetRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DataSetRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateDataSetAsync(DataSet dataSet)
        {
            await _dbContext.DataSets.AddAsync(dataSet);
            await _dbContext.SaveChangesAsync();

            return dataSet.Id;
        }

        public async Task<IEnumerable<DataSet>> GetDataSetsAsync()
        {
            return await _dbContext.DataSets.ToListAsync();
        }
        
        public async Task<ListPageDTO<DataSet>> GetDataSetsAsync(string searchText, int pageNumber, int pageSize, string sortField, int sortOrder)
        {
            var query = _dbContext.DataSets.Where(a => a.Name.Contains(searchText)).Include(a => a.ApplicationUser).AsQueryable();
            
            switch (sortField)
            {
                case DataSetSortField.Id:
                    query = (sortOrder == (int)SortOrder.Asc) ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id);
                    break;
                case DataSetSortField.Name:
                    query = (sortOrder == (int)SortOrder.Asc) ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name);
                    break;
                case DataSetSortField.IsActiveDataSet:
                    query = (sortOrder == (int)SortOrder.Asc) ? query.OrderBy(a => a.IsActiveDataSet) : query.OrderByDescending(a => a.IsActiveDataSet);
                    break;
                case DataSetSortField.AddedByUserFullName:
                    query = (sortOrder == (int)SortOrder.Asc) ? query.OrderBy(a => a.ApplicationUser.FirstName).ThenBy(a => a.ApplicationUser.LastName) : query.OrderByDescending(a => a.ApplicationUser.FirstName).ThenByDescending(a => a.ApplicationUser.LastName);
                    break;
                case DataSetSortField.AddedDate:
                    query = (sortOrder == (int)SortOrder.Asc) ? query.OrderBy(a => a.AddedDate) : query.OrderByDescending(a => a.AddedDate);
                    break;
                default:
                    throw new Exception(ERROR.INVALID_SORT_FIELD_SPECIFIED);
            }

            var dataCount = await query.CountAsync();
            var dataSets = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new ListPageDTO<DataSet>(dataSets, dataCount, pageNumber);
        }

        public async Task<DataSet?> GetDataSetAsync(int id)
        {
            return await _dbContext.DataSets.SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<DataSet?> GetActiveDataSetAsync()
        {
            return await _dbContext.DataSets.FirstOrDefaultAsync(a => a.IsActiveDataSet);
        }

        public async Task UpdateActivityForDataSetAsync(DataSet dataSet)
        {
            _dbContext.Entry(dataSet).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDataSetAsync(DataSet dataSet)
        {
            _dbContext.DataSets.Remove(dataSet);
            await _dbContext.SaveChangesAsync();
        }
    }
}