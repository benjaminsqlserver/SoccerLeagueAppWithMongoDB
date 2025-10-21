// ============================================================================
// FILE: Domain/Entities/MatchEventType.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents types of events that can occur during a match 
    /// (e.g., Goal, Yellow Card, Red Card, Substitution, Penalty).
    /// Used to categorize and track significant moments in matches.
    /// </summary>
    public class MatchEventType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the full name of the event type.
        /// Examples: "Goal", "Yellow Card", "Red Card", "Substitution", "Penalty Scored"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the short code for the event type.
        /// Examples: "GOAL", "YELL", "RED", "SUB", "PENS"
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a description of what this event type represents.
        /// Provides additional context about the event.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon name or emoji used to represent this event in the UI.
        /// Examples: "⚽" for goals, "🟨" for yellow card, "🟥" for red card
        /// Enhances visual recognition of events in match timelines.
        /// </summary>
        public string IconName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color code (hex format) for this event type.
        /// Used for visual styling and quick identification in the interface.
        /// </summary>
        public string ColorCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display order for sorting event types.
        /// Determines the sequence in which events are presented in lists.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this event affects the match score.
        /// True for goals and own goals; false for cards, substitutions, etc.
        /// Used to automatically update match scores when events are recorded.
        /// </summary>
        public bool AffectsScore { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this event affects player discipline records.
        /// True for yellow and red cards; false for goals, substitutions, etc.
        /// Used to track player disciplinary history and suspensions.
        /// </summary>
        public bool AffectsDiscipline { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this event type is active and available.
        /// Inactive event types are preserved for historical data but hidden from new entries.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
