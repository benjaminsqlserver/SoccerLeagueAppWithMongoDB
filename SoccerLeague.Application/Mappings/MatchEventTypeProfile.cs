using AutoMapper;
using SoccerLeague.Application.DTOs.MatchEventType;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class MatchEventTypeProfile : Profile
    {
        public MatchEventTypeProfile()
        {
            CreateMap<MatchEventType, MatchEventTypeDto>();

            CreateMap<CreateMatchEventTypeDto, MatchEventType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateMatchEventTypeDto, MatchEventType>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
