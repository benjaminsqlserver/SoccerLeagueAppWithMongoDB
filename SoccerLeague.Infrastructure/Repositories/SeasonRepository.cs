using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Season entity operations.
    /// Provides specialized queries for seasons including current season, year-based queries, and championship info.
    /// </summary>
    public class SeasonRepository : GenericRepository<Season>, ISeasonRepository
    {
        public SeasonRepository(MongoDbContext context)
            : base(context, "Seasons")
        {
        }

        /// <summary>
        /// Gets a paginated list of seasons with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<Season>> GetSeasonsAsync(SeasonQueryParameters parameters)
        {
            var filterBuilder = Builders<Season>.Filter;
            var filter = filterBuilder.Eq(s => s.IsDeleted, false);

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(parameters.SeasonStatusId))
            {
                filter &= filterBuilder.Eq(s => s.SeasonStatusId, parameters.SeasonStatusId);
            }

            if (parameters.IsCurrentSeason.HasValue)
            {
                filter &= filterBuilder.Eq(s => s.IsCurrentSeason, parameters.IsCurrentSeason.Value);
            }

            if (parameters.StartDateFrom.HasValue)
            {
                filter &= filterBuilder.Gte(s => s.StartDate, parameters.StartDateFrom.Value);
            }

            if (parameters.StartDateTo.HasValue)
            {
                filter &= filterBuilder.Lte(s => s.StartDate, parameters.StartDateTo.Value);
            }

            if (parameters.Year.HasValue)
            {
                var yearStart = new DateTime(parameters.Year.Value, 1, 1);
                var yearEnd = new DateTime(parameters.Year.Value, 12, 31);
                filter &= filterBuilder.Gte(s => s.StartDate, yearStart) &
                         filterBuilder.Lte(s => s.StartDate, yearEnd);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildSeasonSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var seasons = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Season>
            {
                Items = seasons,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets the current active season.
        /// </summary>
        public async Task<Season?> GetCurrentSeasonAsync()
        {
            var filter = Builders<Season>.Filter.Eq(s => s.IsCurrentSeason, true) &
                        Builders<Season>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a season by its name.
        /// </summary>
        public async Task<Season?> GetSeasonByNameAsync(string name)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Name, name) &
                        Builders<Season>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all seasons for a specific year.
        /// </summary>
        public async Task<IReadOnlyList<Season>> GetSeasonsByYearAsync(int year)
        {
            var yearStart = new DateTime(year, 1, 1);
            var yearEnd = new DateTime(year, 12, 31);

            var filter = Builders<Season>.Filter.Gte(s => s.StartDate, yearStart) &
                        Builders<Season>.Filter.Lte(s => s.StartDate, yearEnd) &
                        Builders<Season>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(s => s.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all seasons with a specific status.
        /// </summary>
        public async Task<IReadOnlyList<Season>> GetSeasonsByStatusAsync(string statusId)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.SeasonStatusId, statusId) &
                        Builders<Season>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(s => s.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a season name already exists.
        /// </summary>
        public async Task<bool> SeasonNameExistsAsync(string name, string? excludeId = null)
        {
            var filterBuilder = Builders<Season>.Filter;
            var filter = filterBuilder.Eq(s => s.Name, name) &
                        filterBuilder.Eq(s => s.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(s => s.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Sets a season as the current season and unsets all others.
        /// </summary>
        public async Task<bool> SetCurrentSeasonAsync(string seasonId)
        {
            // First, unset all current seasons
            var unsetFilter = Builders<Season>.Filter.Eq(s => s.IsCurrentSeason, true) &
                             Builders<Season>.Filter.Ne(s => s.Id, seasonId);
            var unsetUpdate = Builders<Season>.Update
                .Set(s => s.IsCurrentSeason, false)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            await _collection.UpdateManyAsync(unsetFilter, unsetUpdate);

            // Then set the specified season as current
            var setFilter = Builders<Season>.Filter.Eq(s => s.Id, seasonId);
            var setUpdate = Builders<Season>.Update
                .Set(s => s.IsCurrentSeason, true)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(setFilter, setUpdate);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Updates championship information for a completed season.
        /// </summary>
        public async Task<bool> UpdateChampionInfoAsync(string seasonId, string? championTeamId, string? runnerUpTeamId, string? topScorerPlayerId)
        {
            var filter = Builders<Season>.Filter.Eq(s => s.Id, seasonId);
            var update = Builders<Season>.Update
                .Set(s => s.ChampionTeamId, championTeamId)
                .Set(s => s.RunnerUpTeamId, runnerUpTeamId)
                .Set(s => s.TopScorerPlayerId, topScorerPlayerId)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Builds search filter for name and description fields.
        /// </summary>
        protected override FilterDefinition<Season> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<Season>.Filter;
            return filter.Or(
                filter.Regex(s => s.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<Season> BuildSeasonSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<Season>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Descending(s => s.StartDate);
            }

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? sortBuilder.Descending(s => s.Name)
                    : sortBuilder.Ascending(s => s.Name),
                "startdate" => sortDescending
                    ? sortBuilder.Descending(s => s.StartDate)
                    : sortBuilder.Ascending(s => s.StartDate),
                "enddate" => sortDescending
                    ? sortBuilder.Descending(s => s.EndDate)
                    : sortBuilder.Ascending(s => s.EndDate),
                "numberofteams" => sortDescending
                    ? sortBuilder.Descending(s => s.NumberOfTeams)
                    : sortBuilder.Ascending(s => s.NumberOfTeams),
                _ => sortBuilder.Descending(s => s.StartDate)
            };
        }
    }
}
