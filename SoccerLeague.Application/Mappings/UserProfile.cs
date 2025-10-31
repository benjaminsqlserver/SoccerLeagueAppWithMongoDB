using AutoMapper;
using SoccerLeague.Application.DTOs.User;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;

namespace SoccerLeague.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.AuthProvider, opt => opt.MapFrom(src => src.AuthProvider.ToString()));

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.AuthProvider, opt => opt.MapFrom(src => AuthenticationProvider.Local))
                .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => new List<string>()));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.AuthProvider, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleId, opt => opt.Ignore());
        }
    }
}
