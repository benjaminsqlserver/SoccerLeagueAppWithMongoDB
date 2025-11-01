using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public GoogleLoginDto GoogleLoginDto { get; set; } = null!;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
