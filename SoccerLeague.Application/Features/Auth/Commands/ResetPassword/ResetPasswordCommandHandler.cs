using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;

namespace SoccerLeague.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IEmailService _emailService;

        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _emailService = emailService;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var validator = new ResetPasswordCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<bool>.Failure(errors);
            }

            var user = await _userRepository.GetByEmailAsync(request.ResetPasswordDto.Email);

            if (user == null)
            {
                return Result<bool>.Failure("Invalid reset token");
            }

            // Verify token
            if (string.IsNullOrEmpty(user.PasswordResetToken) ||
                user.PasswordResetToken != request.ResetPasswordDto.Token ||
                !user.PasswordResetTokenExpiry.HasValue ||
                user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Invalid or expired reset token");
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ResetPasswordDto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            // Terminate all sessions for security
            await _sessionRepository.TerminateAllUserSessionsAsync(user.Id, "Password reset");

            // Send notification email
            await _emailService.SendPasswordChangedNotificationAsync(user.Email, user.FirstName);

            return Result<bool>.Success(true);
        }
    }
}
