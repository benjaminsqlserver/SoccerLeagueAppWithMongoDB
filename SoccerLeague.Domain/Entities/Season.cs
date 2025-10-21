// ============================================================================
// FILE: Domain/Entities/Season.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    public class Season : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., "2024/2025"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SeasonStatusId { get; set; } = string.Empty; // Foreign key to SeasonStatus
        public string Description { get; set; } = string.Empty;
        public int NumberOfTeams { get; set; }
        public int MatchesPerTeam { get; set; }
        public bool IsCurrentSeason { get; set; } = false;

        // Rules
        public int PointsForWin { get; set; } = 3;
        public int PointsForDraw { get; set; } = 1;
        public int PointsForLoss { get; set; } = 0;

        // Champion info (filled after season completion)
        public string? ChampionTeamId { get; set; }
        public string? RunnerUpTeamId { get; set; }
        public string? TopScorerPlayerId { get; set; }
    }
}