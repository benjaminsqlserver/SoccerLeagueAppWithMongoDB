using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Match entity operations.
    /// </summary>
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(MongoDbContext context)
            : base(context, "Matches")
        {
        }

        public async Task<PagedResult<Match>> GetMatchesAsync(MatchQueryParameters parameters)
        {
            var filterBuilder = Builders<Match>.Filter;
            var filter = filterBuilder.Eq(m => m.IsDeleted, false);

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(parameters.SeasonId))
            {
                filter &= filterBuilder.Eq(m => m.SeasonId, parameters.SeasonId);
            }

            if (!string.IsNullOrWhiteSpace(parameters.TeamId))
            {
                filter &= filterBuilder.Or(
                    filterBuilder.Eq(m => m.HomeTeamId, parameters.TeamId),
                    filterBuilder.Eq(m => m.AwayTeamId, parameters.TeamId)
                );
            }

            if (!string.IsNullOrWhiteSpace(parameters.MatchStatusId))
            {
                filter &= filterBuilder.Eq(m => m.MatchStatusId, parameters.MatchStatusId);
            }

            if (parameters.FromDate.HasValue)
            {
                filter &= filterBuilder.Gte(m => m.ScheduledDate, parameters.FromDate.Value);
            }

            if (parameters.ToDate.HasValue)
            {
                filter &= filterBuilder.Lte(m => m.ScheduledDate, parameters.ToDate.Value);
            }

            if (parameters.Round.HasValue)
            {
                filter &= filterBuilder.Eq(m => m.Round, parameters.Round.Value);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Venue))
            {
                filter &= filterBuilder.Regex(m => m.Venue,
                    new MongoDB.Bson.BsonRegularExpression(parameters.Venue, "i"));
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildMatchSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var matches = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Match>
            {
                Items = matches,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        public async Task<IReadOnlyList<Match>> GetMatchesBySeasonAsync(string seasonId)
        {
            var filter = Builders<Match>.Filter.Eq(m => m.SeasonId, seasonId) &
                         Builders<Match>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(m => m.Round)
                .ThenBy(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Match>> GetMatchesByTeamAsync(string teamId)
        {
            var filter = Builders<Match>.Filter.Or(
                Builders<Match>.Filter.Eq(m => m.HomeTeamId, teamId),
                Builders<Match>.Filter.Eq(m => m.AwayTeamId, teamId)
            ) & Builders<Match>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(m => m.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Match>> GetUpcomingMatchesAsync(int count = 10)
        {
            var now = DateTime.UtcNow;
            var filter = Builders<Match>.Filter.Gte(m => m.ScheduledDate, now) &
                         Builders<Match>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(m => m.ScheduledDate)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Match>> GetRecentMatchesAsync(int count = 10)
        {
            var now = DateTime.UtcNow;
            var filter = Builders<Match>.Filter.Lt(m => m.ScheduledDate, now) &
                         Builders<Match>.Filter.Eq(m => m.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(m => m.ScheduledDate)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<Match?> GetMatchWithDetailsAsync(string matchId)
        {
            return await GetByIdAsync(matchId);
        }

        public async Task<bool> AddMatchEventAsync(string matchId, MatchEvent matchEvent)
        {
            var filter = Builders<Match>.Filter.Eq(m => m.Id, matchId);
            var update = Builders<Match>.Update
                .Push(m => m.Events, matchEvent)
                .Set(m => m.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateMatchScoreAsync(string matchId, int? homeScore, int? awayScore)
        {
            var filter = Builders<Match>.Filter.Eq(m => m.Id, matchId);
            var update = Builders<Match>.Update
                .Set(m => m.HomeTeamScore, homeScore)
                .Set(m => m.AwayTeamScore, awayScore)
                .Set(m => m.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        protected override FilterDefinition<Match> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<Match>.Filter;
            return filter.Or(
                filter.Regex(m => m.Venue, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(m => m.Referee, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        private SortDefinition<Match> BuildMatchSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<Match>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Descending(m => m.ScheduledDate);
            }

            return sortBy.ToLower() switch
            {
                "scheduleddate" => sortDescending
                    ? sortBuilder.Descending(m => m.ScheduledDate)
                    : sortBuilder.Ascending(m => m.ScheduledDate),
                "round" => sortDescending
                    ? sortBuilder.Descending(m => m.Round)
                    : sortBuilder.Ascending(m => m.Round),
                "venue" => sortDescending
                    ? sortBuilder.Descending(m => m.Venue)
                    : sortBuilder.Ascending(m => m.Venue),
                _ => sortBuilder.Descending(m => m.ScheduledDate)
            };
        }
    }
}
