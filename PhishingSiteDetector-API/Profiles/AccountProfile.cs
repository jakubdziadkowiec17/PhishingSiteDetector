using AutoMapper;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<ApplicationUser, AccountDTO>().ReverseMap();
            CreateMap<ApplicationUser, AccountDataDTO>();
            CreateMap<LanguageDTO, ApplicationUser>();
        }
    }
}