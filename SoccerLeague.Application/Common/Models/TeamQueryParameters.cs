using System;

namespace SoccerLeague.Application.Common.Models
{
    public class TeamQueryParameters : QueryParameters
    {
        public string? TeamStatusId { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? MinWins { get; set; }
        public int? MinPoints { get; set; }
    }
}
