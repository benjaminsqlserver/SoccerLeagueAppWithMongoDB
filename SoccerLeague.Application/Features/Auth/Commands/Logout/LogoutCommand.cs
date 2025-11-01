using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
    }
}
