using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.UserSessions.Commands.CreateUserSession
{
    public class CreateUserSessionCommandValidator : AbstractValidator<CreateUserSessionCommand>
    {
        private readonly IUserSessionRepository _repository;

        public CreateUserSessionCommandValidator(IUserSessionRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Session.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Session.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .MustAsync(BeUniqueRefreshToken).WithMessage("Refresh token already exists");

            RuleFor(x => x.Session.TokenId)
                .NotEmpty().WithMessage("Token ID is required");

            RuleFor(x => x.Session.SessionExpiryDate)
                .NotEmpty().WithMessage("Session expiry date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Session expiry date must be in the future");
        }

        private async Task<bool> BeUniqueRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            return !await _repository.RefreshTokenExistsAsync(refreshToken);
        }
    }
}
