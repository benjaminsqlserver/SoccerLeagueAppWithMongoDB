using AutoMapper;
using SoccerLeague.Application.DTOs.Team;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, TeamDto>()
                .ForMember(dest => dest.TeamStatusName, opt => opt.Ignore())
                .ForMember(dest => dest.TeamStatusCode, opt => opt.Ignore())
                .ForMember(dest => dest.TeamStatusColorCode, opt => opt.Ignore());

            CreateMap<CreateTeamDto, Team>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.TotalMatches, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Wins, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Draws, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Losses, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.GoalsScored, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.GoalsConceded, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Points, opt => opt.MapFrom(src => 0));

            CreateMap<UpdateTeamDto, Team>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
