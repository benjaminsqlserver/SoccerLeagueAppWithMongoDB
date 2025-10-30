using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface ISeasonStatusRepository : IGenericRepository<SeasonStatus>
    {
        Task<IReadOnlyList<SeasonStatus>> GetActiveStatusesAsync();
        Task<SeasonStatus?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code, string? excludeId = null);
    }
}
