namespace SoccerLeague.Domain.Enums
{
    /// <summary>
    /// Represents the authentication provider used by a user.
    /// </summary>
    public enum AuthenticationProvider
    {
        /// <summary>
        /// Local authentication using email and password.
        /// </summary>
        Local = 0,

        /// <summary>
        /// Google OAuth authentication.
        /// </summary>
        Google = 1,

        /// <summary>
        /// Microsoft OAuth authentication (for future extension).
        /// </summary>
        Microsoft = 2,

        /// <summary>
        /// Facebook OAuth authentication (for future extension).
        /// </summary>
        Facebook = 3
    }
}