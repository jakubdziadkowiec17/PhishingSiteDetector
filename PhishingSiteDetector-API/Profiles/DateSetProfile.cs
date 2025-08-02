using AutoMapper;
using PhishingSiteDetector_API.Models.DTOs;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Profiles
{
    public class DateSetProfile : Profile
    {
        public DateSetProfile()
        {
            CreateMap<DataSet, DataSetItemDTO>();
            CreateMap<DataSetStatusDTO, DataSet>();
        }
    }
}