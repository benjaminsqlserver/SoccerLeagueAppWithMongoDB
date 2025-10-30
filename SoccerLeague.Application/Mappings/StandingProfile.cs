using AutoMapper;
using SoccerLeague.Application.DTOs.Standing;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class StandingProfile : Profile
    {
        public StandingProfile()
        {
            CreateMap<Standing, StandingDto>()
                .ForMember(dest => dest.SeasonName, opt => opt.Ignore())
                .ForMember(dest => dest.TeamName, opt => opt.Ignore())
                .ForMember(dest => dest.TeamLogo, opt => opt.Ignore());

            CreateMap<CreateStandingDto, Standing>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.MatchesPlayed, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Wins, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Draws, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Losses, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.GoalsFor, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.GoalsAgainst, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.RecentForm, opt => opt.MapFrom(src => new List<string>()));

            CreateMap<UpdateStandingDto, Standing>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
