namespace SoccerLeague.Application.DTOs.UserSession
{
    public class TerminateSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public string TerminationReason { get; set; } = string.Empty;
    }
}
