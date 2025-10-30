using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Player entity operations.
    /// Provides specialized queries for players including team-based queries, position-based queries, and statistics.
    /// </summary>
    public class PlayerRepository : GenericRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(MongoDbContext context)
            : base(context, "Players")
        {
        }

        /// <summary>
        /// Gets a paginated list of players with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<Player>> GetPlayersAsync(PlayerQueryParameters parameters)
        {
            var filterBuilder = Builders<Player>.Filter;
            var filter = filterBuilder.Eq(p => p.IsDeleted, false);

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(parameters.TeamId))
            {
                filter &= filterBuilder.Eq(p => p.TeamId, parameters.TeamId);
            }

            if (!string.IsNullOrWhiteSpace(parameters.PlayerPositionId))
            {
                filter &= filterBuilder.Eq(p => p.PlayerPositionId, parameters.PlayerPositionId);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Nationality))
            {
                filter &= filterBuilder.Eq(p => p.Nationality, parameters.Nationality);
            }

            if (parameters.IsActive.HasValue)
            {
                filter &= filterBuilder.Eq(p => p.IsActive, parameters.IsActive.Value);
            }

            if (parameters.MinAge.HasValue || parameters.MaxAge.HasValue)
            {
                var now = DateTime.UtcNow;

                if (parameters.MinAge.HasValue)
                {
                    var maxBirthDate = now.AddYears(-parameters.MinAge.Value);
                    filter &= filterBuilder.Lte(p => p.DateOfBirth, maxBirthDate);
                }

                if (parameters.MaxAge.HasValue)
                {
                    var minBirthDate = now.AddYears(-parameters.MaxAge.Value - 1);
                    filter &= filterBuilder.Gte(p => p.DateOfBirth, minBirthDate);
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.PreferredFoot))
            {
                filter &= filterBuilder.Eq(p => p.PreferredFoot, parameters.PreferredFoot);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildPlayerSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var players = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Player>
            {
                Items = players,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets all players belonging to a specific team.
        /// </summary>
        public async Task<IReadOnlyList<Player>> GetPlayersByTeamAsync(string teamId)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.TeamId, teamId) &
                        Builders<Player>.Filter.Eq(p => p.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(p => p.JerseyNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all players for a specific position.
        /// </summary>
        public async Task<IReadOnlyList<Player>> GetPlayersByPositionAsync(string positionId)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.PlayerPositionId, positionId) &
                        Builders<Player>.Filter.Eq(p => p.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(p => p.Goals)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all active players.
        /// </summary>
        public async Task<IReadOnlyList<Player>> GetActivePlayersAsync()
        {
            var filter = Builders<Player>.Filter.Eq(p => p.IsActive, true) &
                        Builders<Player>.Filter.Eq(p => p.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a player by ID with full details.
        /// </summary>
        public async Task<Player?> GetPlayerWithDetailsAsync(string playerId)
        {
            return await GetByIdAsync(playerId);
        }

        /// <summary>
        /// Checks if a jersey number already exists in a team.
        /// </summary>
        /// <param name="teamId">The team ID</param>
        /// <param name="jerseyNumber">The jersey number to check</param>
        /// <param name="excludePlayerId">Optional player ID to exclude from the check (for updates)</param>
        public async Task<bool> JerseyNumberExistsInTeamAsync(string teamId, int jerseyNumber, string? excludePlayerId = null)
        {
            var filterBuilder = Builders<Player>.Filter;
            var filter = filterBuilder.Eq(p => p.TeamId, teamId) &
                        filterBuilder.Eq(p => p.JerseyNumber, jerseyNumber) &
                        filterBuilder.Eq(p => p.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludePlayerId))
            {
                filter &= filterBuilder.Ne(p => p.Id, excludePlayerId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Gets the top scorers.
        /// </summary>
        public async Task<IReadOnlyList<Player>> GetTopScorersAsync(int count = 10)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(p => p.Goals)
                .ThenByDescending(p => p.Assists)
                .Limit(count)
                .ToListAsync();
        }

        /// <summary>
        /// Updates a player's statistics.
        /// </summary>
        public async Task<bool> UpdatePlayerStatisticsAsync(string playerId, int appearances, int goals, int assists, int yellowCards, int redCards, int minutesPlayed)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, playerId);
            var update = Builders<Player>.Update
                .Inc(p => p.Appearances, appearances)
                .Inc(p => p.Goals, goals)
                .Inc(p => p.Assists, assists)
                .Inc(p => p.YellowCards, yellowCards)
                .Inc(p => p.RedCards, redCards)
                .Inc(p => p.MinutesPlayed, minutesPlayed)
                .Set(p => p.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Builds search filter for name, nickname, and nationality fields.
        /// </summary>
        protected override FilterDefinition<Player> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<Player>.Filter;
            return filter.Or(
                filter.Regex(p => p.FirstName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(p => p.LastName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(p => p.Nickname, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(p => p.Nationality, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<Player> BuildPlayerSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<Player>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Ascending(p => p.LastName).Ascending(p => p.FirstName);
            }

            return sortBy.ToLower() switch
            {
                "firstname" => sortDescending
                    ? sortBuilder.Descending(p => p.FirstName)
                    : sortBuilder.Ascending(p => p.FirstName),
                "lastname" => sortDescending
                    ? sortBuilder.Descending(p => p.LastName)
                    : sortBuilder.Ascending(p => p.LastName),
                "jerseynumber" => sortDescending
                    ? sortBuilder.Descending(p => p.JerseyNumber)
                    : sortBuilder.Ascending(p => p.JerseyNumber),
                "dateofbirth" => sortDescending
                    ? sortBuilder.Descending(p => p.DateOfBirth)
                    : sortBuilder.Ascending(p => p.DateOfBirth),
                "goals" => sortDescending
                    ? sortBuilder.Descending(p => p.Goals)
                    : sortBuilder.Ascending(p => p.Goals),
                "assists" => sortDescending
                    ? sortBuilder.Descending(p => p.Assists)
                    : sortBuilder.Ascending(p => p.Assists),
                "appearances" => sortDescending
                    ? sortBuilder.Descending(p => p.Appearances)
                    : sortBuilder.Ascending(p => p.Appearances),
                _ => sortBuilder.Ascending(p => p.LastName).Ascending(p => p.FirstName)
            };
        }
    }
}
