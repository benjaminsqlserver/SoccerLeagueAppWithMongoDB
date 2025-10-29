namespace SoccerLeague.Application.DTOs.PlayerPosition
{
    public class CreatePlayerPositionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
