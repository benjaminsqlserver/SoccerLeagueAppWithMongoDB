using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    public class StandingRepository : GenericRepository<Standing>, IStandingRepository
    {
        public StandingRepository(MongoDbContext context)
            : base(context, "Standings")
        {
        }

        public async Task<PagedResult<Standing>> GetStandingsAsync(StandingQueryParameters parameters)
        {
            var filterBuilder = Builders<Standing>.Filter;
            var filter = filterBuilder.Eq(s => s.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(parameters.SeasonId))
            {
                filter &= filterBuilder.Eq(s => s.SeasonId, parameters.SeasonId);
            }

            if (!string.IsNullOrWhiteSpace(parameters.TeamId))
            {
                filter &= filterBuilder.Eq(s => s.TeamId, parameters.TeamId);
            }

            if (parameters.MinPosition.HasValue)
            {
                filter &= filterBuilder.Gte(s => s.Position, parameters.MinPosition.Value);
            }

            if (parameters.MaxPosition.HasValue)
            {
                filter &= filterBuilder.Lte(s => s.Position, parameters.MaxPosition.Value);
            }

            if (parameters.MinPoints.HasValue)
            {
                filter &= filterBuilder.Gte(s => s.Points, parameters.MinPoints.Value);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            var totalCount = await _collection.CountDocumentsAsync(filter);
            var sort = BuildStandingSortDefinition(parameters.SortBy, parameters.SortDescending);

            var standings = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Standing>
            {
                Items = standings,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        public async Task<IReadOnlyList<Standing>> GetStandingsBySeasonAsync(string seasonId)
        {
            var filter = Builders<Standing>.Filter.Eq(s => s.SeasonId, seasonId) &
                        Builders<Standing>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(s => s.Position)
                .ToListAsync();
        }

        public async Task<Standing?> GetStandingBySeasonAndTeamAsync(string seasonId, string teamId)
        {
            var filter = Builders<Standing>.Filter.Eq(s => s.SeasonId, seasonId) &
                        Builders<Standing>.Filter.Eq(s => s.TeamId, teamId) &
                        Builders<Standing>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStandingStatisticsAsync(string standingId, int wins, int draws, int losses,
            int goalsFor, int goalsAgainst, int points, string formResult)
        {
            var filter = Builders<Standing>.Filter.Eq(s => s.Id, standingId);

            var update = Builders<Standing>.Update
                .Inc(s => s.MatchesPlayed, 1)
                .Inc(s => s.Wins, wins)
                .Inc(s => s.Draws, draws)
                .Inc(s => s.Losses, losses)
                .Inc(s => s.GoalsFor, goalsFor)
                .Inc(s => s.GoalsAgainst, goalsAgainst)
                .Inc(s => s.Points, points)
                .Push(s => s.RecentForm, formResult)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);

            // Keep only last 5 form results
            var standing = await _collection.Find(filter).FirstOrDefaultAsync();
            if (standing != null && standing.RecentForm.Count > 5)
            {
                var keepForm = standing.RecentForm.TakeLast(5).ToList();
                var formUpdate = Builders<Standing>.Update.Set(s => s.RecentForm, keepForm);
                await _collection.UpdateOneAsync(filter, formUpdate);
            }

            return result.ModifiedCount > 0;
        }

        public async Task<bool> RecalculatePositionsAsync(string seasonId)
        {
            var filter = Builders<Standing>.Filter.Eq(s => s.SeasonId, seasonId) &
                        Builders<Standing>.Filter.Eq(s => s.IsDeleted, false);

            var standings = await _collection.Find(filter)
                .SortByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenByDescending(s => s.GoalsFor)
                .ToListAsync();

            var position = 1;
            foreach (var standing in standings)
            {
                var updateFilter = Builders<Standing>.Filter.Eq(s => s.Id, standing.Id);
                var update = Builders<Standing>.Update
                    .Set(s => s.Position, position)
                    .Set(s => s.ModifiedDate, DateTime.UtcNow);

                await _collection.UpdateOneAsync(updateFilter, update);
                position++;
            }

            return true;
        }

        public async Task<IReadOnlyList<Standing>> GetTopStandingsAsync(string seasonId, int count = 10)
        {
            var filter = Builders<Standing>.Filter.Eq(s => s.SeasonId, seasonId) &
                        Builders<Standing>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(s => s.Position)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<bool> StandingExistsForTeamInSeasonAsync(string seasonId, string teamId, string? excludeId = null)
        {
            var filterBuilder = Builders<Standing>.Filter;
            var filter = filterBuilder.Eq(s => s.SeasonId, seasonId) &
                        filterBuilder.Eq(s => s.TeamId, teamId) &
                        filterBuilder.Eq(s => s.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(s => s.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        protected override FilterDefinition<Standing> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<Standing>.Filter;
            return filter.Empty;
        }

        private SortDefinition<Standing> BuildStandingSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<Standing>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Ascending(s => s.Position);
            }

            return sortBy.ToLower() switch
            {
                "position" => sortDescending
                    ? sortBuilder.Descending(s => s.Position)
                    : sortBuilder.Ascending(s => s.Position),
                "points" => sortDescending
                    ? sortBuilder.Descending(s => s.Points)
                    : sortBuilder.Ascending(s => s.Points),
                "goaldifference" => sortDescending
                    ? sortBuilder.Descending(s => s.GoalDifference)
                    : sortBuilder.Ascending(s => s.GoalDifference),
                "goalsfor" => sortDescending
                    ? sortBuilder.Descending(s => s.GoalsFor)
                    : sortBuilder.Ascending(s => s.GoalsFor),
                "matchesplayed" => sortDescending
                    ? sortBuilder.Descending(s => s.MatchesPlayed)
                    : sortBuilder.Ascending(s => s.MatchesPlayed),
                _ => sortBuilder.Ascending(s => s.Position)
            };
        }
    }
}
