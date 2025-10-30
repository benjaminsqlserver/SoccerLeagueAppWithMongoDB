namespace SoccerLeague.Application.DTOs.Season
{
    public class UpdateSeasonDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SeasonStatusId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfTeams { get; set; }
        public int MatchesPerTeam { get; set; }
        public bool IsCurrentSeason { get; set; }
        public int PointsForWin { get; set; }
        public int PointsForDraw { get; set; }
        public int PointsForLoss { get; set; }
        public string? ChampionTeamId { get; set; }
        public string? RunnerUpTeamId { get; set; }
        public string? TopScorerPlayerId { get; set; }
    }
}
