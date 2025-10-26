using MongoDB.Driver;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for MatchEventType entity operations.
    /// Provides specialized queries for match event types including active, scoring, and disciplinary events.
    /// </summary>
    public class MatchEventTypeRepository : GenericRepository<MatchEventType>, IMatchEventTypeRepository
    {
        public MatchEventTypeRepository(MongoDbContext context)
            : base(context, "MatchEventTypes")
        {
        }

        /// <summary>
        /// Gets all active match event types ordered by display order.
        /// </summary>
        public async Task<IReadOnlyList<MatchEventType>> GetActiveEventTypesAsync()
        {
            var filter = Builders<MatchEventType>.Filter.Eq(m => m.IsActive, true) &
                         Builders<MatchEventType>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a match event type by its unique code.
        /// </summary>
        public async Task<MatchEventType?> GetByCodeAsync(string code)
        {
            var filter = Builders<MatchEventType>.Filter.Eq(m => m.Code, code) &
                         Builders<MatchEventType>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all active match event types that affect score (e.g., goals, penalties).
        /// </summary>
        public async Task<IReadOnlyList<MatchEventType>> GetScoreAffectingTypesAsync()
        {
            var filter = Builders<MatchEventType>.Filter.Eq(m => m.AffectsScore, true) &
                         Builders<MatchEventType>.Filter.Eq(m => m.IsActive, true) &
                         Builders<MatchEventType>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all active match event types that affect discipline (e.g., yellow/red cards).
        /// </summary>
        public async Task<IReadOnlyList<MatchEventType>> GetDisciplineAffectingTypesAsync()
        {
            var filter = Builders<MatchEventType>.Filter.Eq(m => m.AffectsDiscipline, true) &
                         Builders<MatchEventType>.Filter.Eq(m => m.IsActive, true) &
                         Builders<MatchEventType>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a code already exists in the database.
        /// </summary>
        /// <param name="code">The code to check</param>
        /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
        public async Task<bool> CodeExistsAsync(string code, string? excludeId = null)
        {
            var filterBuilder = Builders<MatchEventType>.Filter;
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
        protected override FilterDefinition<MatchEventType> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<MatchEventType>.Filter;
            return filter.Or(
                filter.Regex(m => m.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(m => m.Code, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(m => m.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }
    }
}