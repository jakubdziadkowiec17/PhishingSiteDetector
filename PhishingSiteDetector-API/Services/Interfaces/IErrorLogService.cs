using PhishingSiteDetector_API.Models.DTOs;

namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IErrorLogService
    {
        Task<ResponseDTO?> CreateErrorLogAsync(string message, Exception ex);
    }
}