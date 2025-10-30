namespace SoccerLeague.Application.DTOs.Team
{
    public class CreateTeamDto
    {
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public DateTime FoundedDate { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string TeamStatusId { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
    }
}
