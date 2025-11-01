using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Result<bool>>
    {
        public ResetPasswordDto ResetPasswordDto { get; set; } = null!;
    }
}
