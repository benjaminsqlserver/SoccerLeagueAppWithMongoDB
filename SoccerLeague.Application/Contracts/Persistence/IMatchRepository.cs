using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Contracts.Persistence
{
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Domain.Entities;

    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<PagedResult<Match>> GetMatchesAsync(MatchQueryParameters parameters);
        Task<IReadOnlyList<Match>> GetMatchesBySeasonAsync(string seasonId);
        Task<IReadOnlyList<Match>> GetMatchesByTeamAsync(string teamId);
        Task<IReadOnlyList<Match>> GetUpcomingMatchesAsync(int count = 10);
        Task<IReadOnlyList<Match>> GetRecentMatchesAsync(int count = 10);
        Task<Match?> GetMatchWithDetailsAsync(string matchId);
        Task<bool> AddMatchEventAsync(string matchId, MatchEvent matchEvent);
        Task<bool> UpdateMatchScoreAsync(string matchId, int? homeScore, int? awayScore);
    }
}

