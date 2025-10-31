namespace SoccerLeague.Domain.Constants
{
    /// <summary>
    /// Contains constants for system-defined roles.
    /// These roles are created during database seeding and cannot be deleted.
    /// </summary>
    public static class SystemRoles
    {
        /// <summary>
        /// Administrator role with full system access.
        /// </summary>
        public const string Administrator = "Administrator";

        /// <summary>
        /// Manager role with access to manage league operations.
        /// </summary>
        public const string Manager = "Manager";

        /// <summary>
        /// Referee role with access to match management.
        /// </summary>
        public const string Referee = "Referee";

        /// <summary>
        /// Team Manager role with access to manage their team.
        /// </summary>
        public const string TeamManager = "TeamManager";

        /// <summary>
        /// Regular user role with basic read access.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Fan role with access to view matches and standings.
        /// </summary>
        public const string Fan = "Fan";

        public const string Player = "Player";

        /// <summary>
        /// Gets all system role names.
        /// </summary>
        public static readonly string[] AllRoles =
        {
            Administrator,
            Manager,
            Referee,
            TeamManager,
            User,
            Fan,
            Player
        };
    }
}
