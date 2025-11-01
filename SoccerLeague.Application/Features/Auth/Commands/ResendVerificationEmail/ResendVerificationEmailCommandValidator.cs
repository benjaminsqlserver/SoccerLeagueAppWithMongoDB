using FluentValidation;

namespace SoccerLeague.Application.Features.Auth.Commands.ResendVerificationEmail
{
    public class ResendVerificationEmailCommandValidator : AbstractValidator<ResendVerificationEmailCommand>
    {
        public ResendVerificationEmailCommandValidator()
        {
            RuleFor(x => x.ResendDto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
