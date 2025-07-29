using Microsoft.EntityFrameworkCore;
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

        public async Task<ListPageDTO<DataSet>> GetDataSetsAsync(string searchText, int pageNumber, int pageSize)
        {
            var query = _dbContext.DataSets.Where(a => a.Name.Contains(searchText)).AsQueryable();
            var dataCount = await query.CountAsync();
            var dataSets = await query.OrderByDescending(a => a.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

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