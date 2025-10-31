using AutoMapper;
using SoccerLeague.Application.DTOs.Role;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();

            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ForMember(dest => dest.IsSystemRole, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
