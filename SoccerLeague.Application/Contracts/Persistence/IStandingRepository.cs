using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IStandingRepository : IGenericRepository<Standing>
    {
        Task<PagedResult<Standing>> GetStandingsAsync(StandingQueryParameters parameters);
        Task<IReadOnlyList<Standing>> GetStandingsBySeasonAsync(string seasonId);
        Task<Standing?> GetStandingBySeasonAndTeamAsync(string seasonId, string teamId);
        Task<bool> UpdateStandingStatisticsAsync(string standingId, int wins, int draws, int losses, int goalsFor, int goalsAgainst, int points, string formResult);
        Task<bool> RecalculatePositionsAsync(string seasonId);
        Task<IReadOnlyList<Standing>> GetTopStandingsAsync(string seasonId, int count = 10);
        Task<bool> StandingExistsForTeamInSeasonAsync(string seasonId, string teamId, string? excludeId = null);
    }
}
