using AutoMapper;
using PhishingSiteDetector_API.Models.Domain;
using PhishingSiteDetector_API.Models.DTOs;

namespace PhishingSiteDetector_API.Profiles
{
    public class PredictionProfile : Profile
    {
        public PredictionProfile()
        {
            CreateMap<UrlPrediction, UrlPredictionDTO>()
                .ForMember(dest => dest.IsPhishing, opt => opt.MapFrom(src => src.PredictedLabel))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => Math.Round(src.Probability * 100, 2)));
        }
    }
}