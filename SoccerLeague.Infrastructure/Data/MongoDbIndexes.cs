using MongoDB.Driver;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Data
{
    /// <summary>
    /// Creates MongoDB indexes for improved query performance.
    /// </summary>
    public class MongoDbIndexes
    {
        private readonly MongoDbContext _context;

        public MongoDbIndexes(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates all necessary indexes for the database collections.
        /// </summary>
        public async Task CreateIndexesAsync()
        {
            await CreateMatchIndexesAsync();
            await CreateMatchEventTypeIndexesAsync();
            await CreateTeamIndexesAsync();
            await CreatePlayerIndexesAsync();
            await CreateSeasonIndexesAsync();
            await CreateStandingIndexesAsync();
        }

        private async Task CreateMatchIndexesAsync()
        {
            var indexKeysDefinitions = new[]
            {
                // Index on SeasonId for filtering matches by season
                Builders<Match>.IndexKeys.Ascending(m => m.SeasonId),
                
                // Index on HomeTeamId and AwayTeamId for team-based queries
                Builders<Match>.IndexKeys.Ascending(m => m.HomeTeamId),
                Builders<Match>.IndexKeys.Ascending(m => m.AwayTeamId),
                
                // Index on ScheduledDate for date-based filtering and sorting
                Builders<Match>.IndexKeys.Descending(m => m.ScheduledDate),
                
                // Index on MatchStatusId for filtering by status
                Builders<Match>.IndexKeys.Ascending(m => m.MatchStatusId),
                
                // Index on Round for filtering by matchday
                Builders<Match>.IndexKeys.Ascending(m => m.Round),
                
                // Index on IsDeleted for soft delete filtering
                Builders<Match>.IndexKeys.Ascending(m => m.IsDeleted),
                
                // Compound index for season and scheduled date (common query pattern)
                Builders<Match>.IndexKeys
                    .Ascending(m => m.SeasonId)
                    .Ascending(m => m.ScheduledDate),
                
                // Compound index for team queries
                Builders<Match>.IndexKeys
                    .Ascending(m => m.HomeTeamId)
                    .Ascending(m => m.IsDeleted),

                Builders<Match>.IndexKeys
                    .Ascending(m => m.AwayTeamId)
                    .Ascending(m => m.IsDeleted)
            };

            foreach (var indexKey in indexKeysDefinitions)
            {
                var indexModel = new CreateIndexModel<Match>(indexKey);
                await _context.Matches.Indexes.CreateOneAsync(indexModel);
            }
        }

        private async Task CreateMatchEventTypeIndexesAsync()
        {
            var indexKeysDefinitions = new[]
            {
                // Index on Code for quick lookup
                Builders<MatchEventType>.IndexKeys.Ascending(met => met.Code),
                
                // Index on IsActive for filtering active types
                Builders<MatchEventType>.IndexKeys.Ascending(met => met.IsActive),
                
                // Index on DisplayOrder for sorting
                Builders<MatchEventType>.IndexKeys.Ascending(met => met.DisplayOrder)
            };

            foreach (var indexKey in indexKeysDefinitions)
            {
                var indexModel = new CreateIndexModel<MatchEventType>(indexKey);
                await _context.MatchEventTypes.Indexes.CreateOneAsync(indexModel);
            }
        }

        private async Task CreateTeamIndexesAsync()
        {
            var indexKeysDefinitions = new[]
            {
                Builders<Team>.IndexKeys.Ascending(t => t.Name),
                Builders<Team>.IndexKeys.Ascending(t => t.TeamStatusId),
                Builders<Team>.IndexKeys.Ascending(t => t.IsDeleted)
            };

            foreach (var indexKey in indexKeysDefinitions)
            {
                var indexModel = new CreateIndexModel<Team>(indexKey);
                await _context.Teams.Indexes.CreateOneAsync(indexModel);
            }
        }

        private async Task CreatePlayerIndexesAsync()
        {
            var indexKeysDefinitions = new[]
            {
                Builders<Player>.IndexKeys.Ascending(p => p.TeamId),
                Builders<Player>.IndexKeys.Ascending(p => p.PlayerPositionId),
                Builders<Player>.IndexKeys.Ascending(p => p.IsActive),
                Builders<Player>.IndexKeys.Ascending(p => p.IsDeleted),
                
                // Compound index for team and player queries
                Builders<Player>.IndexKeys
                    .Ascending(p => p.TeamId)
                    .Ascending(p => p.IsDeleted),
                
                // Compound index for name searching
                Builders<Player>.IndexKeys
                    .Ascending(p => p.FirstName)
                    .Ascending(p => p.LastName)
            };

            foreach (var indexKey in indexKeysDefinitions)
            {
                var indexModel = new CreateIndexModel<Player>(indexKey);
                await _context.Players.Indexes.CreateOneAsync(indexModel);
            }
        }

        private async Task CreateSeasonIndexesAsync()
        {
            var indexKeysDefinitions = new[]
            {
                Builders<Season>.IndexKeys.Descending(s => s.StartDate),
                Builders<Season>.IndexKeys.Ascending(s => s.IsCurrentSeason),
                Builders<Season>.IndexKeys.Ascending(s => s.SeasonStatusId),
                Builders<Season>.IndexKeys.Ascending(s => s.IsDeleted),
                
                // Compound index for active season queries
                Builders<Season>.IndexKeys
                    .Ascending(s => s.IsCurrentSeason)
                    .Ascending(s => s.IsDeleted)
            };

            foreach (var indexKey in indexKeysDefinitions)
            {
                var indexModel = new CreateIndexModel<Season>(indexKey);
                await _context.Seasons.Indexes.CreateOneAsync(indexModel);
            }
        }

        private async Task CreateStandingIndexesAsync()
        {
            var indexKeysDefinitions = new[]
            {
        Builders<Standing>.IndexKeys.Ascending(s => s.SeasonId),
        Builders<Standing>.IndexKeys.Ascending(s => s.TeamId),
        Builders<Standing>.IndexKeys.Descending(s => s.Points),
        Builders<Standing>.IndexKeys.Ascending(s => s.Position),
        
        // Compound index for season standings (most common query)
        // Index by Points, GoalsFor, and GoalsAgainst (which together determine ranking)
        Builders<Standing>.IndexKeys
            .Ascending(s => s.SeasonId)
            .Descending(s => s.Points)
            .Descending(s => s.GoalsFor)
            .Ascending(s => s.GoalsAgainst),
        
        // Unique index to ensure one standing per team per season
        Builders<Standing>.IndexKeys
            .Ascending(s => s.SeasonId)
            .Ascending(s => s.TeamId)
    };

            for (int i = 0; i < indexKeysDefinitions.Length; i++)
            {
                var options = new CreateIndexOptions();

                // Make the last index unique (SeasonId + TeamId)
                if (i == indexKeysDefinitions.Length - 1)
                {
                    options.Unique = true;
                }

                var indexModel = new CreateIndexModel<Standing>(indexKeysDefinitions[i], options);
                await _context.Standings.Indexes.CreateOneAsync(indexModel);
            }
        }
    }
}
