using System;

namespace SoccerLeague.Application.DTOs.UserSession
{
    public class UserSessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionExpiryDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public bool IsActive { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceName { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OperatingSystem { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? TerminationReason { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
