using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<PagedResult<Role>> GetRolesAsync(RoleQueryParameters parameters);
        Task<Role?> GetByNameAsync(string name);
        Task<IReadOnlyList<Role>> GetActiveRolesAsync();
        Task<IReadOnlyList<Role>> GetSystemRolesAsync();
        Task<bool> RoleNameExistsAsync(string name, string? excludeId = null);
        Task<bool> IsSystemRoleAsync(string roleId);
        Task<bool> AddPermissionToRoleAsync(string roleId, string permission);
        Task<bool> RemovePermissionFromRoleAsync(string roleId, string permission);
        Task<bool> UpdatePermissionsAsync(string roleId, List<string> permissions);
    }
}
