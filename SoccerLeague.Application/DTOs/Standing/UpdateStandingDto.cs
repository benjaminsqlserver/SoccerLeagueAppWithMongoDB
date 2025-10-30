namespace SoccerLeague.Application.DTOs.Standing
{
    public class UpdateStandingDto
    {
        public string Id { get; set; } = string.Empty;
        public string SeasonId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public int Position { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points { get; set; }
        public List<string> RecentForm { get; set; } = new List<string>();
    }
}
