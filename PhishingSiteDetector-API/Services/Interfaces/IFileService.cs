namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}