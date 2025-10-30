using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface ITeamStatusRepository : IGenericRepository<TeamStatus>
    {
        Task<IReadOnlyList<TeamStatus>> GetActiveStatusesAsync();
        Task<TeamStatus?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code, string? excludeId = null);
    }
}
