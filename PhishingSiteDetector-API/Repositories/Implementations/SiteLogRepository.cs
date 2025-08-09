using Microsoft.EntityFrameworkCore;
using PhishingSiteDetector_API.Database;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;

namespace PhishingSiteDetector_API.Repositories.Implementations
{
    public class SiteLogRepository : ISiteLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SiteLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateSiteLogAsync(SiteLog siteLog)
        {
            await _dbContext.SiteLogs.AddAsync(siteLog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<SiteLog>> GetSiteLogsAsync(int year)
        {
            return await _dbContext.SiteLogs.Where(log => log.AddedDate.Year == year).ToListAsync();
        }
    }
}