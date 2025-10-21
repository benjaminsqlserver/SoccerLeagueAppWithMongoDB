// ============================================================================
// FILE: Domain/Entities/Player.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents a soccer/football player in the league.
    /// Contains personal information, contract details, and performance statistics.
    /// </summary>
    public class Player : BaseEntity
    {
        /// <summary>
        /// Gets or sets the player's first name (given name).
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's last name (family name/surname).
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's nickname or commonly used name.
        /// Example: "CR7" instead of "Cristiano Ronaldo dos Santos Aveiro"
        /// </summary>
        public string Nickname { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's date of birth.
        /// Used to calculate age and verify eligibility for age-restricted competitions.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the player's nationality/citizenship.
        /// Important for international player quotas and eligibility rules.
        /// </summary>
        public string Nationality { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's jersey/shirt number.
        /// Typically ranges from 1-99, must be unique within a team.
        /// </summary>
        public int JerseyNumber { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference to the player's position.
        /// Links to PlayerPosition entity (Goalkeeper, Defender, Midfielder, Forward).
        /// </summary>
        public string PlayerPositionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key reference to the player's current team.
        /// Links to the Team entity the player is registered with.
        /// </summary>
        public string TeamId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's photo/headshot.
        /// Used for displaying player images in rosters and match lineups.
        /// </summary>
        public string Photo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the player's height in centimeters.
        /// Physical attribute used for player profiles and scouting.
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the player's weight in kilograms.
        /// Physical attribute used for player profiles and fitness tracking.
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the player's preferred foot for kicking.
        /// Values: "Left", "Right", or "Both"
        /// Important for tactical decisions and player positioning.
        /// </summary>
        public string PreferredFoot { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the player is currently active.
        /// False if player is injured, suspended, or temporarily unavailable.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date when the player's contract with the team started.
        /// Used for contract management and transfer eligibility.
        /// </summary>
        public DateTime? ContractStartDate { get; set; }

        /// <summary>
        /// Gets or sets the date when the player's contract expires.
        /// Used for contract renewal planning and transfer window management.
        /// </summary>
        public DateTime? ContractEndDate { get; set; }

        // ========================================================================
        // Statistics Section - Career/season performance metrics
        // ========================================================================

        /// <summary>
        /// Gets or sets the total number of match appearances for the player.
        /// Counts all matches where the player was in the starting lineup or came on as substitute.
        /// </summary>
        public int Appearances { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of goals scored by the player.
        /// Does not include own goals or penalty shootout goals.
        /// </summary>
        public int Goals { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of assists provided by the player.
        /// Counted when player's pass directly leads to a goal.
        /// </summary>
        public int Assists { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of yellow cards received.
        /// Used to track disciplinary record and calculate suspensions.
        /// </summary>
        public int YellowCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of red cards received.
        /// Direct red cards or second yellow cards in a match.
        /// </summary>
        public int RedCards { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total minutes played across all appearances.
        /// Used to calculate per-90-minutes statistics and player workload.
        /// </summary>
        public int MinutesPlayed { get; set; } = 0;

        // ========================================================================
        // Computed Properties - Derived values
        // ========================================================================

        /// <summary>
        /// Gets the player's full name by combining first and last names.
        /// Computed property that concatenates FirstName and LastName.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets the player's current age calculated from date of birth.
        /// Computed property that calculates years between birth date and now.
        /// Note: This is a simplified calculation and may be off by 1 year 
        /// if the birthday hasn't occurred yet this year.
        /// </summary>
        public int Age => DateTime.UtcNow.Year - DateOfBirth.Year;
    }
}
