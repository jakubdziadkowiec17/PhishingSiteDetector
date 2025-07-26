using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Repositories.Interfaces
{
    public interface IErrorLogRepository
    {
        Task CreateErrorLogAsync(ErrorLog errorLog);
    }
}