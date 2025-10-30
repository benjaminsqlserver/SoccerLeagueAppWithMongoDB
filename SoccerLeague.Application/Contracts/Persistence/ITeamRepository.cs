using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<PagedResult<Team>> GetTeamsAsync(TeamQueryParameters parameters);
        Task<IReadOnlyList<Team>> GetActiveTeamsAsync();
        Task<IReadOnlyList<Team>> GetTeamsByCityAsync(string city);
        Task<IReadOnlyList<Team>> GetTeamsByCountryAsync(string country);
        Task<Team?> GetTeamWithDetailsAsync(string teamId);
        Task<bool> TeamNameExistsAsync(string name, string? excludeId = null);
        Task<bool> UpdateTeamStatisticsAsync(string teamId, int wins, int draws, int losses, int goalsScored, int goalsConceded, int points);
        Task<IReadOnlyList<Team>> GetTopTeamsByPointsAsync(int count = 10);
    }
}
