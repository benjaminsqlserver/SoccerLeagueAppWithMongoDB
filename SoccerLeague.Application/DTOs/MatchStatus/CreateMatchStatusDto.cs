namespace SoccerLeague.Application.DTOs.MatchStatus
{
    public class CreateMatchStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
