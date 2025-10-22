using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Common.Models
{
    public class MatchQueryParameters : QueryParameters
    {
        public string? SeasonId { get; set; }
        public string? TeamId { get; set; }
        public string? MatchStatusId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Round { get; set; }
        public string? Venue { get; set; }
    }
}

