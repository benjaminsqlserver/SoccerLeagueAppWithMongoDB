using System;

namespace SoccerLeague.Application.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string AuthProvider { get; set; } = string.Empty;
        public string? GoogleId { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsGoogleUser { get; set; }
        public List<string> RoleIds { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}