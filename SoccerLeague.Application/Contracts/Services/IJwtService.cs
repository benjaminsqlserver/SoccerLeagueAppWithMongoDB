using SoccerLeague.Domain.Entities;
using System.Security.Claims;

namespace SoccerLeague.Application.Contracts.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, List<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        DateTime GetTokenExpiry();
    }
}
