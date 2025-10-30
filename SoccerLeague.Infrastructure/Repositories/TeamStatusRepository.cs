using MongoDB.Driver;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for TeamStatus entity operations.
    /// Provides specialized queries for team statuses including active statuses and code lookups.
    /// </summary>
    public class TeamStatusRepository : GenericRepository<TeamStatus>, ITeamStatusRepository
    {
        public TeamStatusRepository(MongoDbContext context)
            : base(context, "TeamStatuses")
        {
        }

        /// <summary>
        /// Gets all active team statuses ordered by display order.
        /// </summary>
        public async Task<IReadOnlyList<TeamStatus>> GetActiveStatusesAsync()
        {
            var filter = Builders<TeamStatus>.Filter.Eq(t => t.IsActive, true) &
                        Builders<TeamStatus>.Filter.Eq(t => t.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(t => t.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a team status by its unique code.
        /// </summary>
        public async Task<TeamStatus?> GetByCodeAsync(string code)
        {
            var filter = Builders<TeamStatus>.Filter.Eq(t => t.Code, code) &
                        Builders<TeamStatus>.Filter.Eq(t => t.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks if a code already exists in the database.
        /// </summary>
        /// <param name="code">The code to check</param>
        /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
        public async Task<bool> CodeExistsAsync(string code, string? excludeId = null)
        {
            var filterBuilder = Builders<TeamStatus>.Filter;
            var filter = filterBuilder.Eq(t => t.Code, code) &
                        filterBuilder.Eq(t => t.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(t => t.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Builds search filter for name, code, and description fields.
        /// </summary>
        protected override FilterDefinition<TeamStatus> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<TeamStatus>.Filter;
            return filter.Or(
                filter.Regex(t => t.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(t => t.Code, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(t => t.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }
    }
}
