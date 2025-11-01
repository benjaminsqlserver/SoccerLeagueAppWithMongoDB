using AutoMapper;
using SoccerLeague.Application.DTOs.UserSession;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class UserSessionProfile : Profile
    {
        public UserSessionProfile()
        {
            CreateMap<UserSession, UserSessionDto>();

            CreateMap<CreateUserSessionDto, UserSession>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SessionStartDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastActivityDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.TerminationReason, opt => opt.Ignore())
                .ForMember(dest => dest.TerminationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
