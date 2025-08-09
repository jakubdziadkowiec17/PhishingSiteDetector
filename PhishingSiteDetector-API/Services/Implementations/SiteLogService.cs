using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Repositories.Interfaces;
using PhishingSiteDetector_API.Services.Interfaces;

namespace PhishingSiteDetector_API.Services.Implementations
{
    public class SiteLogService : ISiteLogService
    {
        private readonly ISiteLogRepository _siteLogRepository;
        public SiteLogService(ISiteLogRepository siteLogRepository)
        {
            _siteLogRepository = siteLogRepository;
        }

        public async Task<List<ChartDTO>> GetSiteLogsAsync(int year)
        {
            var siteLogs = await _siteLogRepository.GetSiteLogsAsync(year);

            var result = Enumerable.Range(1, 12).Select(month =>
            {
                var count = siteLogs.Count(log =>
                    log.AddedDate.Year == year &&
                    log.AddedDate.Month == month);

                return new ChartDTO
                {
                    Month = month,
                    Count = count
                };
            }).ToList();

            return result;
        }
    }
}