using AutoMapper;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Profiles
{
    public class DateSetProfile : Profile
    {
        public DateSetProfile()
        {
            CreateMap<DataSet, DataSetItemDTO>()
                .ForMember(dest => dest.AddedByUserFullName, opt => opt.MapFrom(src => src.ApplicationUser.FirstName + " " + src.ApplicationUser.LastName));
            CreateMap<DataSetStatusDTO, DataSet>();
        }
    }
}