// ============================================================================
// FILE: Domain/Entities/SeasonStatus.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents the status of a season (e.g., Not Started, In Progress, Completed, Cancelled).
    /// Tracks the lifecycle of a league season from planning through completion.
    /// </summary>
    public class SeasonStatus : BaseEntity
    {
        /// <summary>
        /// Gets or sets the full name of the season status.
        /// Examples: "Not Started", "In Progress", "Completed", "Cancelled"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the abbreviated code for the status.
        /// Examples: "NST" (Not Started), "INP" (In Progress), "COM" (Completed)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description explaining the season status.
        /// Provides context about what this status means for the season.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color code (hex format) for UI display.
        /// Enables visual identification of season status in dashboards and reports.
        /// </summary>
        public string ColorCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display order for sorting status options.
        /// Controls the sequence in selection lists and filters.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets whether this status is active and available for use.
        /// Inactive statuses are retained for historical seasons but hidden from new selections.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
