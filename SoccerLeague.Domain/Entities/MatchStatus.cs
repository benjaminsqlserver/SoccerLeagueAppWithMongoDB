// ============================================================================
// FILE: Domain/Entities/MatchStatus.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents the status of a match (e.g., Scheduled, In Progress, Completed, Postponed, Cancelled).
    /// This lookup entity tracks the lifecycle state of matches.
    /// </summary>
    public class MatchStatus : BaseEntity
    {
        /// <summary>
        /// Gets or sets the full name of the match status.
        /// Examples: "Scheduled", "In Progress", "Completed", "Postponed", "Cancelled"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the short code/abbreviation for the status.
        /// Examples: "SCH" (Scheduled), "INP" (In Progress), "COM" (Completed)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a description explaining what this status means.
        /// Provides clarity on the state of the match.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color code (hex format) for UI display purposes.
        /// Enables visual distinction between different statuses in the user interface.
        /// Example: "#22c55e" for green (In Progress), "#ef4444" for red (Cancelled)
        /// </summary>
        public string ColorCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display order for sorting status lists.
        /// Lower numbers appear first in dropdown lists and filters.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this status is active.
        /// Inactive statuses are not available for new matches but preserve historical data.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
