using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<PagedResult<User>> GetUsersAsync(UserQueryParameters parameters);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<IReadOnlyList<User>> GetActiveUsersAsync();
        Task<IReadOnlyList<User>> GetLockedOutUsersAsync();
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleId);
        Task<bool> EmailExistsAsync(string email, string? excludeId = null);
        Task<bool> UpdatePasswordAsync(string userId, string passwordHash);
        Task<bool> UpdateLastLoginAsync(string userId);
        Task<bool> IncrementAccessFailedCountAsync(string userId);
        Task<bool> ResetAccessFailedCountAsync(string userId);
        Task<bool> LockoutUserAsync(string userId, DateTime lockoutEnd);
        Task<bool> UnlockUserAsync(string userId);
        Task<bool> ConfirmEmailAsync(string userId);
    }
}
