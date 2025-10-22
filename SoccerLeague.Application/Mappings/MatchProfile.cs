using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Mappings
{
    using AutoMapper;
    using SoccerLeague.Application.DTOs.Match;
    using SoccerLeague.Domain.Entities;
    

    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            // Match mappings
            CreateMap<Match, MatchDto>()
                .ForMember(dest => dest.SeasonName, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeamName, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeamLogo, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeamName, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeamLogo, opt => opt.Ignore())
                .ForMember(dest => dest.MatchStatusName, opt => opt.Ignore())
                .ForMember(dest => dest.MatchStatusCode, opt => opt.Ignore())
                .ForMember(dest => dest.MatchStatusColorCode, opt => opt.Ignore());

            CreateMap<CreateMatchDto, Match>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeamScore, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeamScore, opt => opt.Ignore())
                .ForMember(dest => dest.HomeTeamHalfTimeScore, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeamHalfTimeScore, opt => opt.Ignore())
                .ForMember(dest => dest.Attendance, opt => opt.Ignore())
                .ForMember(dest => dest.WeatherConditions, opt => opt.Ignore())
                .ForMember(dest => dest.ActualKickoffTime, opt => opt.Ignore())
                .ForMember(dest => dest.FullTimeTime, opt => opt.Ignore());

            CreateMap<UpdateMatchDto, Match>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            // MatchEvent mappings
            CreateMap<MatchEvent, MatchEventDto>()
                .ForMember(dest => dest.MatchEventTypeName, opt => opt.Ignore())
                .ForMember(dest => dest.MatchEventTypeCode, opt => opt.Ignore())
                .ForMember(dest => dest.MatchEventTypeIcon, opt => opt.Ignore())
                .ForMember(dest => dest.MatchEventTypeColorCode, opt => opt.Ignore());

            CreateMap<CreateMatchEventDto, MatchEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));
        }
    }
}

