using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public LoginDto LoginDto { get; set; } = null!;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
