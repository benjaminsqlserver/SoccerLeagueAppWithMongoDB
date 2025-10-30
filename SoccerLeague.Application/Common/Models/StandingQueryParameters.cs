using System;

namespace SoccerLeague.Application.Common.Models
{
    public class StandingQueryParameters : QueryParameters
    {
        public string? SeasonId { get; set; }
        public string? TeamId { get; set; }
        public int? MinPosition { get; set; }
        public int? MaxPosition { get; set; }
        public int? MinPoints { get; set; }
    }
}
