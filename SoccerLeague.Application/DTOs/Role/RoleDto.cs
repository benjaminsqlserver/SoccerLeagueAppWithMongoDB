using System;
using System.Collections.Generic;

namespace SoccerLeague.Application.DTOs.Role
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public string? Metadata { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
