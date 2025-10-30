using System;

namespace SoccerLeague.Application.Common.Models
{
    public class SeasonQueryParameters : QueryParameters
    {
        public string? SeasonStatusId { get; set; }
        public bool? IsCurrentSeason { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public int? Year { get; set; }
    }
}
