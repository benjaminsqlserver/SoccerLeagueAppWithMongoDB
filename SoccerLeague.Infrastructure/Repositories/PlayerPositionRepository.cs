using MongoDB.Driver;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for PlayerPosition entity operations.
    /// Provides specialized queries for player positions including active positions and code lookups.
    /// </summary>
    public class PlayerPositionRepository : GenericRepository<PlayerPosition>, IPlayerPositionRepository
    {
        public PlayerPositionRepository(MongoDbContext context)
            : base(context, "PlayerPositions")
        {
        }

        /// <summary>
        /// Gets all active player positions ordered by display order.
        /// </summary>
        public async Task<IReadOnlyList<PlayerPosition>> GetActivePositionsAsync()
        {
            var filter = Builders<PlayerPosition>.Filter.Eq(p => p.IsActive, true) &
                        Builders<PlayerPosition>.Filter.Eq(p => p.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(p => p.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a player position by its unique code.
        /// </summary>
        public async Task<PlayerPosition?> GetByCodeAsync(string code)
        {
            var filter = Builders<PlayerPosition>.Filter.Eq(p => p.Code, code) &
                        Builders<PlayerPosition>.Filter.Eq(p => p.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks if a code already exists in the database.
        /// </summary>
        /// <param name="code">The code to check</param>
        /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
        public async Task<bool> CodeExistsAsync(string code, string? excludeId = null)
        {
            var filterBuilder = Builders<PlayerPosition>.Filter;
            var filter = filterBuilder.Eq(p => p.Code, code) &
                        filterBuilder.Eq(p => p.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(p => p.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Builds search filter for name, code, and description fields.
        /// </summary>
        protected override FilterDefinition<PlayerPosition> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<PlayerPosition>.Filter;
            return filter.Or(
                filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(p => p.Code, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }
    }
}
