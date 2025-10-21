// ============================================================================
// FILE: Domain/Entities/Team.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents a soccer/football team in the league.
    /// Contains team information, contact details, and aggregate statistics.
    /// </summary>
    public class Team : BaseEntity
    {
        /// <summary>
        /// Gets or sets the full official name of the team.
        /// Example: "Manchester United Football Club"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the shortened/abbreviated name of the team.
        /// Used for compact displays. Example: "Man Utd"
        /// </summary>
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the team's logo/badge image.
        /// Used for displaying team branding in the UI.
        /// </summary>
        public string Logo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the team was founded/established.
        /// Historical information about the team's origins.
        /// </summary>
        public DateTime FoundedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the team's home stadium/venue.
        /// Where the team plays its home matches.
        /// </summary>
        public string Stadium { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city where the team is based.
        /// Geographic location of the team.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country where the team is located.
        /// National affiliation of the team.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the current team manager/head coach.
        /// Person responsible for team strategy and player selection.
        /// </summary>
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key reference to the team's current status.
        /// Links to the TeamStatus entity (Active, Inactive, Suspended).
        /// </summary>
        public string TeamStatusId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the official contact email address for the team.
        /// Used for communications and administrative purposes.
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the official contact phone number for the team.
        /// Used for communications and emergency contact.
        /// </summary>
        public string? ContactPhone { get; set; }

        // ========================================================================
        // Statistics Section - Aggregate data across all matches
        // ========================================================================

        /// <summary>
        /// Gets or sets the total number of matches played by the team.
        /// Includes wins, draws, and losses across all competitions.
        /// </summary>
        public int TotalMatches { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of matches won by the team.
        /// Updated after each completed match.
        /// </summary>
        public int Wins { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of matches drawn by the team.
        /// Matches that ended with equal scores.
        /// </summary>
        public int Draws { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of matches lost by the team.
        /// Updated after each completed match.
        /// </summary>
        public int Losses { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of goals scored by the team.
        /// Cumulative count of all goals across all matches.
        /// </summary>
        public int GoalsScored { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of goals conceded (allowed) by the team.
        /// Goals scored against the team by opponents.
        /// </summary>
        public int GoalsConceded { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total points accumulated by the team.
        /// Calculated based on wins and draws (typically 3 points for win, 1 for draw).
        /// </summary>
        public int Points { get; set; } = 0;

        // ========================================================================
        // Computed Properties - Calculated values based on statistics
        // ========================================================================

        /// <summary>
        /// Gets the goal difference (goals scored minus goals conceded).
        /// Positive value indicates more goals scored than conceded.
        /// Important metric for league standings when teams are tied on points.
        /// </summary>
        public int GoalDifference => GoalsScored - GoalsConceded;
    }
}
