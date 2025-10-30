using AutoMapper;
using SoccerLeague.Application.DTOs.Player;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.PlayerPositionName, opt => opt.Ignore())
                .ForMember(dest => dest.PlayerPositionCode, opt => opt.Ignore())
                .ForMember(dest => dest.TeamName, opt => opt.Ignore())
                .ForMember(dest => dest.TeamLogo, opt => opt.Ignore());

            CreateMap<CreatePlayerDto, Player>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Appearances, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Assists, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.YellowCards, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.RedCards, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.MinutesPlayed, opt => opt.MapFrom(src => 0));

            CreateMap<UpdatePlayerDto, Player>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
