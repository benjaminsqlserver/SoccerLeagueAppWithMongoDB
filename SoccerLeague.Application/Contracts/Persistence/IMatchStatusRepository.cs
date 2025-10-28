using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IMatchStatusRepository : IGenericRepository<MatchStatus>
    {
        Task<IReadOnlyList<MatchStatus>> GetActiveStatusesAsync();
        Task<MatchStatus?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code, string? excludeId = null);
    }
}
