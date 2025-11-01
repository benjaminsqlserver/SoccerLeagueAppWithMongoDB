using System;
using SoccerLeague.Domain.Enums;

namespace SoccerLeague.Application.Common.Models
{
    public class AuditLogQueryParameters : QueryParameters
    {
        public string? UserId { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public AuditActionType? ActionType { get; set; }
        public bool? Success { get; set; }
        public DateTime? ActionDateFrom { get; set; }
        public DateTime? ActionDateTo { get; set; }
        public string? IpAddress { get; set; }
    }
}
