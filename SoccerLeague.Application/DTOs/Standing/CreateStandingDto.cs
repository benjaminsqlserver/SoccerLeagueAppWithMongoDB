namespace SoccerLeague.Application.DTOs.Standing
{
    public class CreateStandingDto
    {
        public string SeasonId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}
