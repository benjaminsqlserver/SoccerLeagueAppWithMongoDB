// ============================================================================
// FILE: Domain/Entities/PlayerPosition.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents a player position in soccer/football (e.g., Goalkeeper, Defender, Midfielder, Forward).
    /// This is a lookup/reference entity used to categorize players by their playing position.
    /// </summary>
    public class PlayerPosition : BaseEntity
    {
        /// <summary>
        /// Gets or sets the full name of the position.
        /// Examples: "Goalkeeper", "Defender", "Midfielder", "Forward"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the short code/abbreviation for the position.
        /// Examples: "GK" (Goalkeeper), "DF" (Defender), "MF" (Midfielder), "FW" (Forward)
        /// Used for compact displays and data entry.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a detailed description of the position's role and responsibilities.
        /// Provides context about what players in this position typically do.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the order in which this position should be displayed in lists.
        /// Lower numbers appear first. Typically: GK=1, DF=2, MF=3, FW=4
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this position is currently active and selectable.
        /// Inactive positions are hidden from selection but preserved for historical data.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
