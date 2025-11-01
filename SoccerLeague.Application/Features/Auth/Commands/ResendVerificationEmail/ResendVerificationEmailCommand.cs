using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.ResendVerificationEmail
{
    public class ResendVerificationEmailCommand : IRequest<Result<bool>>
    {
        public ResendVerificationEmailDto ResendDto { get; set; } = null!;
    }
}
