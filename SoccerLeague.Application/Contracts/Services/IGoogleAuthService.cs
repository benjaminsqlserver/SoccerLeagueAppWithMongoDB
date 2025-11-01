namespace SoccerLeague.Application.Contracts.Services
{
    public class GoogleUserInfo
    {
        public string Sub { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
    }

    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateGoogleTokenAsync(string idToken);
    }
}
