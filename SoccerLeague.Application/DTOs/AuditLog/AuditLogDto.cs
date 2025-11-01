using System;

namespace SoccerLeague.Application.DTOs.AuditLog
{
    public class AuditLogDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? EntityName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime ActionDate { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? AdditionalData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
