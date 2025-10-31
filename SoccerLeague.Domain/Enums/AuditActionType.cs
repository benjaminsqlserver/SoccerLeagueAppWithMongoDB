namespace SoccerLeague.Domain.Enums
{
    /// <summary>
    /// Represents the type of action performed in an audit log entry.
    /// </summary>
    public enum AuditActionType
    {
        /// <summary>
        /// Entity was created.
        /// </summary>
        Create = 0,

        /// <summary>
        /// Entity was updated.
        /// </summary>
        Update = 1,

        /// <summary>
        /// Entity was deleted.
        /// </summary>
        Delete = 2,

        /// <summary>
        /// User logged in.
        /// </summary>
        Login = 3,

        /// <summary>
        /// User logged out.
        /// </summary>
        Logout = 4,

        /// <summary>
        /// Failed login attempt.
        /// </summary>
        LoginFailed = 5,

        /// <summary>
        /// Password was changed.
        /// </summary>
        PasswordChanged = 6,

        /// <summary>
        /// Password reset was requested.
        /// </summary>
        PasswordResetRequested = 7,

        /// <summary>
        /// Email was confirmed.
        /// </summary>
        EmailConfirmed = 8,

        /// <summary>
        /// User was locked out.
        /// </summary>
        UserLockedOut = 9,

        /// <summary>
        /// User lockout was removed.
        /// </summary>
        UserUnlocked = 10,

        /// <summary>
        /// User role was assigned.
        /// </summary>
        RoleAssigned = 11,

        /// <summary>
        /// User role was removed.
        /// </summary>
        RoleRemoved = 12,

        /// <summary>
        /// Entity was viewed/accessed.
        /// </summary>
        View = 13,

        /// <summary>
        /// System event occurred.
        /// </summary>
        SystemEvent = 14
    }
}
