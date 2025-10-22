using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.DTOs.Match
{
    public class CreateMatchDto
    {
        public string SeasonId { get; set; } = string.Empty;
        public string HomeTeamId { get; set; } = string.Empty;
        public string AwayTeamId { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public int Round { get; set; }
        public string MatchStatusId { get; set; } = string.Empty;
        public string? Referee { get; set; }
        public string? AssistantReferee1 { get; set; }
        public string? AssistantReferee2 { get; set; }
        public string? FourthOfficial { get; set; }
        public string? Notes { get; set; }
    }
}

