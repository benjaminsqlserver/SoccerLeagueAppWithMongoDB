// ============================================================================
// FILE: Domain/Entities/Standing.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents the league standing/table for a specific season
    /// This is typically calculated/updated after each match
    /// </summary>
    public class Standing : BaseEntity
    {
        public string SeasonId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public int Position { get; set; }
        public int MatchesPlayed { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int GoalsFor { get; set; } = 0;
        public int GoalsAgainst { get; set; } = 0;
        public int Points { get; set; } = 0;

        // Form (last 5 matches: W, D, L)
        public List<string> RecentForm { get; set; } = new List<string>(); // e.g., ["W", "W", "D", "L", "W"]

        // Computed properties
        public int GoalDifference => GoalsFor - GoalsAgainst;

        public decimal PointsPerGame => MatchesPlayed > 0 ? (decimal)Points / MatchesPlayed : 0;

        public decimal WinPercentage => MatchesPlayed > 0 ? (decimal)Wins / MatchesPlayed * 100 : 0;
    }
}
