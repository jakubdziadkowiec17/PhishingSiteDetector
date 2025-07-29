using PhishingSiteDetector_API.Models.DTOs;

namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface ISiteLogService
    {
        Task<List<ChartDTO>> GetSiteLogsAsync(int year);
    }
}