using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IUserSessionRepository : IGenericRepository<UserSession>
    {
        Task<PagedResult<UserSession>> GetSessionsAsync(UserSessionQueryParameters parameters);
        Task<UserSession?> GetByRefreshTokenAsync(string refreshToken);
        Task<UserSession?> GetByTokenIdAsync(string tokenId);
        Task<IReadOnlyList<UserSession>> GetActiveSessionsByUserAsync(string userId);
        Task<IReadOnlyList<UserSession>> GetAllSessionsByUserAsync(string userId);
        Task<bool> TerminateSessionAsync(string sessionId, string terminationReason);
        Task<bool> TerminateAllUserSessionsAsync(string userId, string terminationReason);
        Task<bool> UpdateLastActivityAsync(string sessionId);
        Task<bool> RefreshTokenExistsAsync(string refreshToken);
        Task<int> GetActiveSessionCountByUserAsync(string userId);
        Task<bool> CleanupExpiredSessionsAsync();
    }
}
