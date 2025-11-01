using FluentValidation;

namespace SoccerLeague.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(x => x.GoogleLoginDto.IdToken)
                .NotEmpty().WithMessage("Google ID token is required");
        }
    }
}
