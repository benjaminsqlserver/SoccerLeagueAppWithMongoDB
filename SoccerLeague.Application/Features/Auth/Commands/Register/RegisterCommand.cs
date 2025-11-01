using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<Result<AuthResponseDto>>
    {
        public RegisterDto RegisterDto { get; set; } = null!;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
