using AutoMapper;
using SoccerLeague.Application.DTOs.PlayerPosition;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class PlayerPositionProfile : Profile
    {
        public PlayerPositionProfile()
        {
            CreateMap<PlayerPosition, PlayerPositionDto>();

            CreateMap<CreatePlayerPositionDto, PlayerPosition>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdatePlayerPositionDto, PlayerPosition>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}

