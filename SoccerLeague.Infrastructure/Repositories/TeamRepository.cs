using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Team entity operations.
    /// Provides specialized queries for teams including active teams, city/country-based queries, and statistics.
    /// </summary>
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(MongoDbContext context)
            : base(context, "Teams")
        {
        }

        /// <summary>
        /// Gets a paginated list of teams with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<Team>> GetTeamsAsync(TeamQueryParameters parameters)
        {
            var filterBuilder = Builders<Team>.Filter;
            var filter = filterBuilder.Eq(t => t.IsDeleted, false);

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(parameters.TeamStatusId))
            {
                filter &= filterBuilder.Eq(t => t.TeamStatusId, parameters.TeamStatusId);
            }

            if (!string.IsNullOrWhiteSpace(parameters.City))
            {
                filter &= filterBuilder.Eq(t => t.City, parameters.City);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Country))
            {
                filter &= filterBuilder.Eq(t => t.Country, parameters.Country);
            }

            if (parameters.MinWins.HasValue)
            {
                filter &= filterBuilder.Gte(t => t.Wins, parameters.MinWins.Value);
            }

            if (parameters.MinPoints.HasValue)
            {
                filter &= filterBuilder.Gte(t => t.Points, parameters.MinPoints.Value);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildTeamSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var teams = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Team>
            {
                Items = teams,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets all active teams.
        /// </summary>
        public async Task<IReadOnlyList<Team>> GetActiveTeamsAsync()
        {
            var filter = Builders<Team>.Filter.Eq(t => t.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all teams in a specific city.
        /// </summary>
        public async Task<IReadOnlyList<Team>> GetTeamsByCityAsync(string city)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.City, city) &
                        Builders<Team>.Filter.Eq(t => t.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all teams in a specific country.
        /// </summary>
        public async Task<IReadOnlyList<Team>> GetTeamsByCountryAsync(string country)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.Country, country) &
                        Builders<Team>.Filter.Eq(t => t.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a team by ID with full details.
        /// </summary>
        public async Task<Team?> GetTeamWithDetailsAsync(string teamId)
        {
            return await GetByIdAsync(teamId);
        }

        /// <summary>
        /// Checks if a team name already exists.
        /// </summary>
        public async Task<bool> TeamNameExistsAsync(string name, string? excludeId = null)
        {
            var filterBuilder = Builders<Team>.Filter;
            var filter = filterBuilder.Eq(t => t.Name, name) &
                        filterBuilder.Eq(t => t.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(t => t.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Updates a team's statistics.
        /// </summary>
        public async Task<bool> UpdateTeamStatisticsAsync(string teamId, int wins, int draws, int losses, int goalsScored, int goalsConceded, int points)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.Id, teamId);
            var update = Builders<Team>.Update
                .Inc(t => t.TotalMatches, 1)
                .Inc(t => t.Wins, wins)
                .Inc(t => t.Draws, draws)
                .Inc(t => t.Losses, losses)
                .Inc(t => t.GoalsScored, goalsScored)
                .Inc(t => t.GoalsConceded, goalsConceded)
                .Inc(t => t.Points, points)
                .Set(t => t.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Gets the top teams by points.
        /// </summary>
        public async Task<IReadOnlyList<Team>> GetTopTeamsByPointsAsync(int count = 10)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsScored)
                .Limit(count)
                .ToListAsync();
        }

        /// <summary>
        /// Builds search filter for name, short name, city, and stadium fields.
        /// </summary>
        protected override FilterDefinition<Team> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<Team>.Filter;
            return filter.Or(
                filter.Regex(t => t.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(t => t.ShortName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(t => t.City, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(t => t.Stadium, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<Team> BuildTeamSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<Team>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Ascending(t => t.Name);
            }

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? sortBuilder.Descending(t => t.Name)
                    : sortBuilder.Ascending(t => t.Name),
                "shortname" => sortDescending
                    ? sortBuilder.Descending(t => t.ShortName)
                    : sortBuilder.Ascending(t => t.ShortName),
                "city" => sortDescending
                    ? sortBuilder.Descending(t => t.City)
                    : sortBuilder.Ascending(t => t.City),
                "points" => sortDescending
                    ? sortBuilder.Descending(t => t.Points)
                    : sortBuilder.Ascending(t => t.Points),
                "wins" => sortDescending
                    ? sortBuilder.Descending(t => t.Wins)
                    : sortBuilder.Ascending(t => t.Wins),
                "goalsscored" => sortDescending
                    ? sortBuilder.Descending(t => t.GoalsScored)
                    : sortBuilder.Ascending(t => t.GoalsScored),
                "foundeddate" => sortDescending
                    ? sortBuilder.Descending(t => t.FoundedDate)
                    : sortBuilder.Ascending(t => t.FoundedDate),
                _ => sortBuilder.Ascending(t => t.Name)
            };
        }
    }
}
