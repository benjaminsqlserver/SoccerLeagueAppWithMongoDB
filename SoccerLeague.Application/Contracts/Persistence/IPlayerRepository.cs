using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IPlayerRepository : IGenericRepository<Player>
    {
        Task<PagedResult<Player>> GetPlayersAsync(PlayerQueryParameters parameters);
        Task<IReadOnlyList<Player>> GetPlayersByTeamAsync(string teamId);
        Task<IReadOnlyList<Player>> GetPlayersByPositionAsync(string positionId);
        Task<IReadOnlyList<Player>> GetActivePlayersAsync();
        Task<Player?> GetPlayerWithDetailsAsync(string playerId);
        Task<bool> JerseyNumberExistsInTeamAsync(string teamId, int jerseyNumber, string? excludePlayerId = null);
        Task<IReadOnlyList<Player>> GetTopScorersAsync(int count = 10);
        Task<bool> UpdatePlayerStatisticsAsync(string playerId, int appearances, int goals, int assists, int yellowCards, int redCards, int minutesPlayed);
    }
}
