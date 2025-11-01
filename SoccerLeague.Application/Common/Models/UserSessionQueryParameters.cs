using System;

namespace SoccerLeague.Application.Common.Models
{
    public class UserSessionQueryParameters : QueryParameters
    {
        public string? UserId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? SessionStartFrom { get; set; }
        public DateTime? SessionStartTo { get; set; }
        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
    }
}
