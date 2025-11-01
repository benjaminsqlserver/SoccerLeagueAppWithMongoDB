using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoccerLeague.Application.Contracts.Services;

namespace SoccerLeague.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _frontendUrl;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly bool _enableSsl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";

            // SMTP Configuration
            _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
            _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
            _senderEmail = configuration["Email:SenderEmail"] ?? "noreply@soccerleague.com";
            _senderName = configuration["Email:SenderName"] ?? "Soccer League";
            _enableSsl = bool.Parse(configuration["Email:EnableSsl"] ?? "true");
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = _enableSsl;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_senderEmail, _senderName);
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(mailMessage);
                        _logger.LogInformation("Email sent successfully to {Email} with subject '{Subject}'", toEmail, subject);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject '{Subject}'", toEmail, subject);
                return false;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(string email, string firstName, string verificationToken)
        {
            try
            {
                var verificationUrl = $"{_frontendUrl}/verify-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(verificationToken)}";

                var subject = "Verify Your Email - Soccer League";
                var body = $@"
                    <h2>Hello {firstName},</h2>
                    <p>Thank you for registering with Soccer League!</p>
                    <p>Please verify your email address by clicking the link below:</p>
                    <p><a href='{verificationUrl}'>Verify Email Address</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not create an account, please ignore this email.</p>
                    <br/>
                    <p>Best regards,<br/>Soccer League Team</p>
                ";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email verification to {Email}", email);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string firstName, string resetToken)
        {
            try
            {
                var resetUrl = $"{_frontendUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(resetToken)}";

                var subject = "Reset Your Password - Soccer League";
                var body = $@"
                    <h2>Hello {firstName},</h2>
                    <p>We received a request to reset your password.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetUrl}'>Reset Password</a></p>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <br/>
                    <p>Best regards,<br/>Soccer League Team</p>
                ";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string firstName)
        {
            try
            {
                var subject = "Welcome to Soccer League!";
                var body = $@"
                    <h2>Welcome {firstName}!</h2>
                    <p>Your email has been successfully verified.</p>
                    <p>You can now enjoy all the features of Soccer League.</p>
                    <p>Visit our platform: <a href='{_frontendUrl}'>{_frontendUrl}</a></p>
                    <br/>
                    <p>Best regards,<br/>Soccer League Team</p>
                ";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
                return false;
            }
        }

        public async Task<bool> SendPasswordChangedNotificationAsync(string email, string firstName)
        {
            try
            {
                var subject = "Password Changed - Soccer League";
                var body = $@"
                    <h2>Hello {firstName},</h2>
                    <p>Your password has been successfully changed.</p>
                    <p>If you did not make this change, please contact us immediately.</p>
                    <br/>
                    <p>Best regards,<br/>Soccer League Team</p>
                ";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password changed notification to {Email}", email);
                return false;
            }
        }
    }
}