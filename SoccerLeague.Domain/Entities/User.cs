namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;
    using SoccerLeague.Domain.Enums;

    /// <summary>
    /// Represents a user in the system with authentication and profile information.
    /// Supports both email/password and Google OAuth authentication.
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user's email address (unique identifier).
        /// Used for login and communication.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hashed password.
        /// Null for users who authenticate via Google OAuth only.
        /// </summary>
        public string? PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the user's profile picture URL.
        /// picture provided by Google.
        /// </summary>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Picture Provided by user when registering directly
        /// </summary>
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the email has been verified.
        /// </summary>
        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the phone number has been verified.
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether two-factor authentication is enabled.
        /// </summary>
        public bool TwoFactorEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user account is locked out.
        /// Prevents login attempts when true.
        /// </summary>
        public bool LockoutEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time when the lockout ends (UTC).
        /// Null if account is not locked out.
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts.
        /// Used for account lockout policy.
        /// </summary>
        public int AccessFailedCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// Inactive users cannot log in.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time of the last login (UTC).
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets the authentication provider type.
        /// </summary>
        public AuthenticationProvider AuthProvider { get; set; } = AuthenticationProvider.Local;

        /// <summary>
        /// Gets or sets the Google User ID (sub claim) for Google authenticated users.
        /// Null for local authentication users.
        /// </summary>
        public string? GoogleId { get; set; }

        /// <summary>
        /// Gets or sets the email verification token.
        /// Used for email confirmation process.
        /// </summary>
        public string? EmailVerificationToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the email verification token.
        /// </summary>
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        /// <summary>
        /// Gets or sets the password reset token.
        /// Used for password recovery process.
        /// </summary>
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the password reset token.
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for JWT authentication.
        /// Used to obtain new access tokens.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for the refresh token.
        /// </summary>
        public DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>
        /// Gets or sets the collection of role IDs assigned to this user.
        /// </summary>
        public List<string> RoleIds { get; set; } = new List<string>();

        //========================================================================
        // Computed Properties
        //========================================================================

        /// <summary>
        /// Gets the user's full name.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets a value indicating whether the account is currently locked out.
        /// </summary>
        public bool IsLockedOut => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

        /// <summary>
        /// Gets a value indicating whether this is a Google authenticated user.
        /// </summary>
        public bool IsGoogleUser => AuthProvider == AuthenticationProvider.Google && !string.IsNullOrEmpty(GoogleId);
    }
}