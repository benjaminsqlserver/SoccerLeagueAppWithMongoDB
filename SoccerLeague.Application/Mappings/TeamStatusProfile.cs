using AutoMapper;
using SoccerLeague.Application.DTOs.TeamStatus;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class TeamStatusProfile : Profile
    {
        public TeamStatusProfile()
        {
            CreateMap<TeamStatus, TeamStatusDto>();

            CreateMap<CreateTeamStatusDto, TeamStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateTeamStatusDto, TeamStatus>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
