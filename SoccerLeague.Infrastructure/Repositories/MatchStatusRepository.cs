using MongoDB.Driver;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for MatchStatus entity operations.
    /// Provides specialized queries for match statuses including active statuses and code lookups.
    /// </summary>
    public class MatchStatusRepository : GenericRepository<MatchStatus>, IMatchStatusRepository
    {
        public MatchStatusRepository(MongoDbContext context)
            : base(context, "MatchStatuses")
        {
        }

        /// <summary>
        /// Gets all active match statuses ordered by display order.
        /// </summary>
        public async Task<IReadOnlyList<MatchStatus>> GetActiveStatusesAsync()
        {
            var filter = Builders<MatchStatus>.Filter.Eq(m => m.IsActive, true) &
                        Builders<MatchStatus>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a match status by its unique code.
        /// </summary>
        public async Task<MatchStatus?> GetByCodeAsync(string code)
        {
            var filter = Builders<MatchStatus>.Filter.Eq(m => m.Code, code) &
                        Builders<MatchStatus>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks if a code already exists in the database.
        /// </summary>
        /// <param name="code">The code to check</param>
        /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
        public async Task<bool> CodeExistsAsync(string code, string? excludeId = null)
        {
            var filterBuilder = Builders<MatchStatus>.Filter;
            var filter = filterBuilder.Eq(m => m.Code, code) &
                        filterBuilder.Eq(m => m.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(m => m.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Builds search filter for name, code, and description fields.
        /// </summary>
        protected override FilterDefinition<MatchStatus> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<MatchStatus>.Filter;
            return filter.Or(
                filter.Regex(m => m.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(m => m.Code, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(m => m.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }
    }
}
