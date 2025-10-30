namespace SoccerLeague.Application.DTOs.SeasonStatus
{
    public class CreateSeasonStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}

