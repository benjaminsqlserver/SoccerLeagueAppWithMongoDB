using System;

namespace SoccerLeague.Application.DTOs.Standing
{
    public class StandingDto
    {
        public string Id { get; set; } = string.Empty;

        // Season Information
        public string SeasonId { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;

        // Team Information
        public string TeamId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string TeamLogo { get; set; } = string.Empty;

        // Position
        public int Position { get; set; }

        // Match Statistics
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }

        // Goal Statistics
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }

        // Points
        public int Points { get; set; }

        // Performance Metrics
        public decimal PointsPerGame { get; set; }
        public decimal WinPercentage { get; set; }

        // Form
        public List<string> RecentForm { get; set; } = new List<string>();

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
