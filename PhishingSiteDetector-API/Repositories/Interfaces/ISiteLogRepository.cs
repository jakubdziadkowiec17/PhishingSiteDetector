using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Repositories.Interfaces
{
    public interface ISiteLogRepository
    {
        Task CreateSiteLogAsync(SiteLog siteLog);
    }
}