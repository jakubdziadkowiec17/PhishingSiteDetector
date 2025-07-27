using PhishingSiteDetector_API.Models.DTOs;

namespace PhishingSiteDetector_API.Services.Interfaces
{
    public interface IUrlPredictionService
    {
        UrlPredictionDTO Predict(UrlDTO urlDTO);
    }
}