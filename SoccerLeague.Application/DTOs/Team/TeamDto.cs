using System;

namespace SoccerLeague.Application.DTOs.Team
{
    public class TeamDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public DateTime FoundedDate { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;

        // Status Information
        public string TeamStatusId { get; set; } = string.Empty;
        public string TeamStatusName { get; set; } = string.Empty;
        public string TeamStatusCode { get; set; } = string.Empty;
        public string TeamStatusColorCode { get; set; } = string.Empty;

        // Contact Information
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        // Statistics
        public int TotalMatches { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }
        public int Points { get; set; }
        public int GoalDifference { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
