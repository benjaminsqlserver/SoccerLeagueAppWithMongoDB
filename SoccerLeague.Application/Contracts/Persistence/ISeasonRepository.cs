using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface ISeasonRepository : IGenericRepository<Season>
    {
        Task<PagedResult<Season>> GetSeasonsAsync(SeasonQueryParameters parameters);
        Task<Season?> GetCurrentSeasonAsync();
        Task<Season?> GetSeasonByNameAsync(string name);
        Task<IReadOnlyList<Season>> GetSeasonsByYearAsync(int year);
        Task<IReadOnlyList<Season>> GetSeasonsByStatusAsync(string statusId);
        Task<bool> SeasonNameExistsAsync(string name, string? excludeId = null);
        Task<bool> SetCurrentSeasonAsync(string seasonId);
        Task<bool> UpdateChampionInfoAsync(string seasonId, string? championTeamId, string? runnerUpTeamId, string? topScorerPlayerId);
    }
}
