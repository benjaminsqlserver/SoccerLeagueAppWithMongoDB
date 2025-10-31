using System;

namespace SoccerLeague.Application.Common.Models
{
    public class UserQueryParameters : QueryParameters
    {
        public bool? EmailConfirmed { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsLockedOut { get; set; }
        public string? AuthProvider { get; set; }
        public DateTime? LastLoginFrom { get; set; }
        public DateTime? LastLoginTo { get; set; }
    }
}
