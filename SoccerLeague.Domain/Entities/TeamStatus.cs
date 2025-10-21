// ============================================================================
// FILE: Domain/Entities/TeamStatus.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents the operational status of a team (e.g., Active, Inactive, Suspended).
    /// Used to track whether a team is currently participating in the league.
    /// </summary>
    public class TeamStatus : BaseEntity
    {
        /// <summary>
        /// Gets or sets the full name of the team status.
        /// Examples: "Active", "Inactive", "Suspended"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the short code for the status.
        /// Examples: "ACT" (Active), "INA" (Inactive), "SUS" (Suspended)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a description of what this status means for a team.
        /// Clarifies the implications of the status.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color code (hex format) for UI visualization.
        /// Allows status to be easily identified through color coding in the interface.
        /// </summary>
        public string ColorCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display order for listing team statuses.
        /// Determines the sequence in dropdowns and filter options.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this status is currently in use.
        /// Inactive statuses are hidden but maintain referential integrity.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
