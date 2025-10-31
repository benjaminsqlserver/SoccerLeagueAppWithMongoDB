namespace SoccerLeague.Domain.Constants
{
    /// <summary>
    /// Contains constants for permission codes used in the system.
    /// Permissions are assigned to roles to control access to features.
    /// </summary>
    public static class Permissions
    {
        // User Management Permissions
        public const string ViewUsers = "users.view";
        public const string CreateUsers = "users.create";
        public const string UpdateUsers = "users.update";
        public const string DeleteUsers = "users.delete";
        public const string ManageUserRoles = "users.manage-roles";

        // Role Management Permissions
        public const string ViewRoles = "roles.view";
        public const string CreateRoles = "roles.create";
        public const string UpdateRoles = "roles.update";
        public const string DeleteRoles = "roles.delete";

        // Match Management Permissions
        public const string ViewMatches = "matches.view";
        public const string CreateMatches = "matches.create";
        public const string UpdateMatches = "matches.update";
        public const string DeleteMatches = "matches.delete";
        public const string ManageMatchEvents = "matches.manage-events";

        // Team Management Permissions
        public const string ViewTeams = "teams.view";
        public const string CreateTeams = "teams.create";
        public const string UpdateTeams = "teams.update";
        public const string DeleteTeams = "teams.delete";

        // Player Management Permissions
        public const string ViewPlayers = "players.view";
        public const string CreatePlayers = "players.create";
        public const string UpdatePlayers = "players.update";
        public const string DeletePlayers = "players.delete";

        // Season Management Permissions
        public const string ViewSeasons = "seasons.view";
        public const string CreateSeasons = "seasons.create";
        public const string UpdateSeasons = "seasons.update";
        public const string DeleteSeasons = "seasons.delete";

        // Standing Management Permissions
        public const string ViewStandings = "standings.view";
        public const string UpdateStandings = "standings.update";

        // System Administration Permissions
        public const string ViewAuditLogs = "system.view-audit-logs";
        public const string ManageSettings = "system.manage-settings";
        public const string ViewReports = "system.view-reports";

        /// <summary>
        /// Gets all permission codes in the system.
        /// </summary>
        public static readonly string[] AllPermissions =
        {
            ViewUsers, CreateUsers, UpdateUsers, DeleteUsers, ManageUserRoles,
            ViewRoles, CreateRoles, UpdateRoles, DeleteRoles,
            ViewMatches, CreateMatches, UpdateMatches, DeleteMatches, ManageMatchEvents,
            ViewTeams, CreateTeams, UpdateTeams, DeleteTeams,
            ViewPlayers, CreatePlayers, UpdatePlayers, DeletePlayers,
            ViewSeasons, CreateSeasons, UpdateSeasons, DeleteSeasons,
            ViewStandings, UpdateStandings,
            ViewAuditLogs, ManageSettings, ViewReports
        };
    }
}