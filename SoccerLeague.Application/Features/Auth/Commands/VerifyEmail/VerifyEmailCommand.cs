using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommand : IRequest<Result<bool>>
    {
        public VerifyEmailDto VerifyEmailDto { get; set; } = null!;
    }
}
