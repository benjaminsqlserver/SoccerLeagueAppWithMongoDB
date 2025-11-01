using AutoMapper;
using SoccerLeague.Application.DTOs.AuditLog;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile()
        {
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ActionType.ToString()));

            CreateMap<CreateAuditLogDto, AuditLog>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
