using AutoMapper;
using SoccerLeague.Application.DTOs.SeasonStatus;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class SeasonStatusProfile : Profile
    {
        public SeasonStatusProfile()
        {
            CreateMap<SeasonStatus, SeasonStatusDto>();

            CreateMap<CreateSeasonStatusDto, SeasonStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateSeasonStatusDto, SeasonStatus>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
