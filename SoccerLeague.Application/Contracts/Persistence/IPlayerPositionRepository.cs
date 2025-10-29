using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IPlayerPositionRepository : IGenericRepository<PlayerPosition>
    {
        Task<IReadOnlyList<PlayerPosition>> GetActivePositionsAsync();
        Task<PlayerPosition?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code, string? excludeId = null);
    }
}
