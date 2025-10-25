namespace SoccerLeague.Application.DTOs.MatchEventType
{
    public class CreateMatchEventTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool AffectsScore { get; set; }
        public bool AffectsDiscipline { get; set; }
    }
}