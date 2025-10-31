using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserRepository _repository;

        public CreateUserCommandValidator(IUserRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.User.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters")
                .MustAsync(BeUniqueEmail).WithMessage("Email already exists");

            RuleFor(x => x.User.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

            RuleFor(x => x.User.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.User.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.User.PhoneNumber)
                .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.User.PhoneNumber))
                .WithMessage("Phone number cannot exceed 20 characters");
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return !await _repository.EmailExistsAsync(email);
        }
    }
}
