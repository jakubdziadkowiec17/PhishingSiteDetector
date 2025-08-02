using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;
using PhishingSiteDetector_API.Repositories.Interfaces;
using PhishingSiteDetector_API.Services.Interfaces;
using System.Security.Claims;

namespace PhishingSiteDetector_API.Services.Implementations
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IErrorLogRepository _errorLogRepository;
        public ErrorLogService(IHttpContextAccessor httpContextAccessor, IErrorLogRepository errorLogRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseDTO?> CreateErrorLogAsync(string message, Exception ex)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var errorLog = new ErrorLog
                {
                    Message = message,
                    StackTrace = ex.StackTrace,
                    UserId = userId,
                    CreationDate = DateTime.Now
                };

                await _errorLogRepository.CreateErrorLogAsync(errorLog);

                return new ResponseDTO(message);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}