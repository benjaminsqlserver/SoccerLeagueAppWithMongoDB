using MongoDB.Driver;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for SeasonStatus entity operations.
    /// Provides specialized queries for season statuses including active statuses and code lookups.
    /// </summary>
    public class SeasonStatusRepository : GenericRepository<SeasonStatus>, ISeasonStatusRepository
    {
        public SeasonStatusRepository(MongoDbContext context)
            : base(context, "SeasonStatuses")
        {
        }

        /// <summary>
        /// Gets all active season statuses ordered by display order.
        /// </summary>
        public async Task<IReadOnlyList<SeasonStatus>> GetActiveStatusesAsync()
        {
            var filter = Builders<SeasonStatus>.Filter.Eq(s => s.IsActive, true) &
                        Builders<SeasonStatus>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a season status by its unique code.
        /// </summary>
        public async Task<SeasonStatus?> GetByCodeAsync(string code)
        {
            var filter = Builders<SeasonStatus>.Filter.Eq(s => s.Code, code) &
                        Builders<SeasonStatus>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks if a code already exists in the database.
        /// </summary>
        /// <param name="code">The code to check</param>
        /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
        public async Task<bool> CodeExistsAsync(string code, string? excludeId = null)
        {
            var filterBuilder = Builders<SeasonStatus>.Filter;
            var filter = filterBuilder.Eq(s => s.Code, code) &
                        filterBuilder.Eq(s => s.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(s => s.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Builds search filter for name, code, and description fields.
        /// </summary>
        protected override FilterDefinition<SeasonStatus> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<SeasonStatus>.Filter;
            return filter.Or(
                filter.Regex(s => s.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.Code, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }
    }
}
