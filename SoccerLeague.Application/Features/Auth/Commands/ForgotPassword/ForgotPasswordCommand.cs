using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<Result<bool>>
    {
        public ForgotPasswordDto ForgotPasswordDto { get; set; } = null!;
    }
}
