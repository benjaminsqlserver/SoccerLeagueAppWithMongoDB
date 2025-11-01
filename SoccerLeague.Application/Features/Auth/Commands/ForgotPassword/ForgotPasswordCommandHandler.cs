using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;

namespace SoccerLeague.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var validator = new ForgotPasswordCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<bool>.Failure(errors);
            }

            var user = await _userRepository.GetByEmailAsync(request.ForgotPasswordDto.Email);

            // Don't reveal if user exists or not (security best practice)
            if (user == null)
            {
                return Result<bool>.Success(true);
            }

            // Generate reset token
            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateAsync(user);

            // Send password reset email
            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.FirstName,
                resetToken);

            return Result<bool>.Success(true);
        }
    }
}
