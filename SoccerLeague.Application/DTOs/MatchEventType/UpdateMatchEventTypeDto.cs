namespace SoccerLeague.Application.DTOs.MatchEventType
{
    public class UpdateMatchEventTypeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool AffectsScore { get; set; }
        public bool AffectsDiscipline { get; set; }
        public bool IsActive { get; set; }
    }
}
