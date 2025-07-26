namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IErrorLogService
    {
        Task CreateErrorLogAsync(Exception ex);
    }
}