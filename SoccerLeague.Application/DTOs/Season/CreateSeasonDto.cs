namespace SoccerLeague.Application.DTOs.Season
{
    public class CreateSeasonDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SeasonStatusId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfTeams { get; set; }
        public int MatchesPerTeam { get; set; }
        public int PointsForWin { get; set; } = 3;
        public int PointsForDraw { get; set; } = 1;
        public int PointsForLoss { get; set; } = 0;
    }
}
