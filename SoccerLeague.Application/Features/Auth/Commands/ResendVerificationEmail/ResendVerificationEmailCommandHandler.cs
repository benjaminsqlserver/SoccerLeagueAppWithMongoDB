using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;

namespace SoccerLeague.Application.Features.Auth.Commands.ResendVerificationEmail
{
    public class ResendVerificationEmailCommandHandler : IRequestHandler<ResendVerificationEmailCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public ResendVerificationEmailCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Result<bool>> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
        {
            var validator = new ResendVerificationEmailCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<bool>.Failure(errors);
            }

            var user = await _userRepository.GetByEmailAsync(request.ResendDto.Email);

            // Don't reveal if user exists or not (security best practice)
            if (user == null)
            {
                return Result<bool>.Success(true);
            }

            // Check if email is already verified
            if (user.EmailConfirmed)
            {
                return Result<bool>.Failure("Email is already verified");
            }

            // Generate new verification token
            var newToken = Guid.NewGuid().ToString();
            user.EmailVerificationToken = newToken;
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _userRepository.UpdateAsync(user);

            // Send verification email
            await _emailService.SendEmailVerificationAsync(
                user.Email,
                user.FirstName,
                newToken);

            return Result<bool>.Success(true);
        }
    }
}
