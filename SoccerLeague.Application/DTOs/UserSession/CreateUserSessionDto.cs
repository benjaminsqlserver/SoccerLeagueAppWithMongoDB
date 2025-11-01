namespace SoccerLeague.Application.DTOs.UserSession
{
    public class CreateUserSessionDto
    {
        public string UserId { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public DateTime SessionExpiryDate { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceName { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OperatingSystem { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
