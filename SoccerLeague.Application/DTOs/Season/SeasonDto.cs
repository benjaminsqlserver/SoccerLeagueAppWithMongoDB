using System;

namespace SoccerLeague.Application.DTOs.Season
{
    public class SeasonDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Status Information
        public string SeasonStatusId { get; set; } = string.Empty;
        public string SeasonStatusName { get; set; } = string.Empty;
        public string SeasonStatusCode { get; set; } = string.Empty;
        public string SeasonStatusColorCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int NumberOfTeams { get; set; }
        public int MatchesPerTeam { get; set; }
        public bool IsCurrentSeason { get; set; }

        // Rules
        public int PointsForWin { get; set; }
        public int PointsForDraw { get; set; }
        public int PointsForLoss { get; set; }

        // Champion Information
        public string? ChampionTeamId { get; set; }
        public string? ChampionTeamName { get; set; }
        public string? RunnerUpTeamId { get; set; }
        public string? RunnerUpTeamName { get; set; }
        public string? TopScorerPlayerId { get; set; }
        public string? TopScorerPlayerName { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
