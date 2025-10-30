using AutoMapper;
using SoccerLeague.Application.DTOs.Season;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class SeasonProfile : Profile
    {
        public SeasonProfile()
        {
            CreateMap<Season, SeasonDto>()
                .ForMember(dest => dest.SeasonStatusName, opt => opt.Ignore())
                .ForMember(dest => dest.SeasonStatusCode, opt => opt.Ignore())
                .ForMember(dest => dest.SeasonStatusColorCode, opt => opt.Ignore())
                .ForMember(dest => dest.ChampionTeamName, opt => opt.Ignore())
                .ForMember(dest => dest.RunnerUpTeamName, opt => opt.Ignore())
                .ForMember(dest => dest.TopScorerPlayerName, opt => opt.Ignore());

            CreateMap<CreateSeasonDto, Season>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsCurrentSeason, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ChampionTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.RunnerUpTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.TopScorerPlayerId, opt => opt.Ignore());

            CreateMap<UpdateSeasonDto, Season>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
