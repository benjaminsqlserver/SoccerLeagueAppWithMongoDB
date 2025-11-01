namespace SoccerLeague.Application.Contracts.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailVerificationAsync(string email, string firstName, string verificationToken);
        Task<bool> SendPasswordResetEmailAsync(string email, string firstName, string resetToken);
        Task<bool> SendWelcomeEmailAsync(string email, string firstName);
        Task<bool> SendPasswordChangedNotificationAsync(string email, string firstName);
    }
}
