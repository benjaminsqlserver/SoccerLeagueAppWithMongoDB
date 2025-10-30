using System;

namespace SoccerLeague.Application.Common.Models
{
    public class PlayerQueryParameters : QueryParameters
    {
        public string? TeamId { get; set; }
        public string? PlayerPositionId { get; set; }
        public string? Nationality { get; set; }
        public bool? IsActive { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? PreferredFoot { get; set; }
    }
}
