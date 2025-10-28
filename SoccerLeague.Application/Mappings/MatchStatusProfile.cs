using AutoMapper;
using SoccerLeague.Application.DTOs.MatchStatus;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class MatchStatusProfile : Profile
    {
        public MatchStatusProfile()
        {
            CreateMap<MatchStatus, MatchStatusDto>();

            CreateMap<CreateMatchStatusDto, MatchStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateMatchStatusDto, MatchStatus>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
