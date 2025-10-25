

using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IMatchEventTypeRepository : IGenericRepository<MatchEventType>
    {
        Task<IReadOnlyList<MatchEventType>> GetActiveEventTypesAsync();
        Task<MatchEventType?> GetByCodeAsync(string code);
        Task<IReadOnlyList<MatchEventType>> GetScoreAffectingTypesAsync();
        Task<IReadOnlyList<MatchEventType>> GetDisciplineAffectingTypesAsync();
        Task<bool> CodeExistsAsync(string code, string? excludeId = null);
    }
}