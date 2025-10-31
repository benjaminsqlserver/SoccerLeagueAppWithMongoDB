using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        private readonly IUserRepository _repository;

        public ChangePasswordCommandValidator(IUserRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.PasswordData.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .MustAsync(UserExists).WithMessage("User does not exist");

            RuleFor(x => x.PasswordData.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.PasswordData.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .NotEqual(x => x.PasswordData.CurrentPassword).WithMessage("New password must be different from current password");
        }

        private async Task<bool> UserExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}
