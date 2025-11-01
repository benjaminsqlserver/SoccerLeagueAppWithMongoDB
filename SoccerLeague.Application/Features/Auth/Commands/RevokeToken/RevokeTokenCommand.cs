using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommand : IRequest<Result<bool>>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
