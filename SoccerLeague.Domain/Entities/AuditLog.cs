namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;
    using SoccerLeague.Domain.Enums;

    /// <summary>
    /// Represents an audit log entry for tracking user actions and system events.
    /// Used for security, compliance, and debugging purposes.
    /// </summary>
    public class AuditLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user ID who performed the action.
        /// Null for system-generated events.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who performed the action.
        /// Stored for historical reference even if user is deleted.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the type of action performed.
        /// </summary>
        public AuditActionType ActionType { get; set; }

        /// <summary>
        /// Gets or sets the entity type that was affected.
        /// Examples: "User", "Role", "Match", "Team"
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the entity that was affected.
        /// </summary>
        public string? EntityId { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity that was affected.
        /// Stored for quick reference without joining.
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// Gets or sets a description of the action performed.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the old values before the change (JSON format).
        /// Only populated for Update actions.
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// Gets or sets the new values after the change (JSON format).
        /// Populated for Create and Update actions.
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the action was performed.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the action was performed (UTC).
        /// </summary>
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether the action was successful.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Gets or sets the error message if the action failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets additional metadata in JSON format.
        /// Can store custom properties relevant to the audit entry.
        /// </summary>
        public string? AdditionalData { get; set; }
    }
}
