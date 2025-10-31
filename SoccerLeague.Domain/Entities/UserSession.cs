namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents an active user session for tracking and security purposes.
    /// Stores information about user login sessions including device and location data.
    /// </summary>
    public class UserSession : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user ID this session belongs to.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token for this session.
        /// Used to obtain new access tokens.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JWT token ID (jti claim).
        /// </summary>
        public string TokenId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the session was created (UTC).
        /// </summary>
        public DateTime SessionStartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the session expires (UTC).
        /// </summary>
        public DateTime SessionExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last activity in this session (UTC).
        /// </summary>
        public DateTime LastActivityDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether this session is active.
        /// False when user logs out or session is terminated.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the IP address from which the session was created.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string (browser/app information).
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the device type (Web, Mobile, Tablet, Desktop).
        /// </summary>
        public string? DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the device name/identifier.
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the browser name.
        /// </summary>
        public string? BrowserName { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        public string? BrowserVersion { get; set; }

        /// <summary>
        /// Gets or sets the operating system.
        /// </summary>
        public string? OperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets the city from which the session was created.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the country from which the session was created.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the reason for session termination.
        /// Examples: "Logout", "Token Expired", "Admin Revoked", "Security Concern"
        /// </summary>
        public string? TerminationReason { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the session was terminated (UTC).
        /// </summary>
        public DateTime? TerminationDate { get; set; }
    }
}