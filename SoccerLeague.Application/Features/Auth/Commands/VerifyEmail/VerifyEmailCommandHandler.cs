using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;

namespace SoccerLeague.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public VerifyEmailCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Result<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.VerifyEmailDto.Email);

            if (user == null)
            {
                return Result<bool>.Failure("Invalid verification token");
            }

            if (user.EmailConfirmed)
            {
                return Result<bool>.Failure("Email already verified");
            }

            // Verify token
            if (string.IsNullOrEmpty(user.EmailVerificationToken) ||
                user.EmailVerificationToken != request.VerifyEmailDto.Token ||
                !user.EmailVerificationTokenExpiry.HasValue ||
                user.EmailVerificationTokenExpiry.Value < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Invalid or expired verification token");
            }

            // Confirm email
            await _userRepository.ConfirmEmailAsync(user.Id);

            // Send welcome email
            await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

            return Result<bool>.Success(true);
        }
    }
}
