using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;

namespace PhishingSiteDetector_API.Repositories.Implementations
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ErrorLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateErrorLogAsync(ErrorLog errorLog)
        {
            await _dbContext.ErrorLogs.AddAsync(errorLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}