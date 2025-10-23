using MongoDB.Driver;
using SoccerLeague.Domain.Constants;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Data
{
    /// <summary>
    /// Seeds initial data into the MongoDB database.
    /// Populates lookup tables with predefined values.
    /// </summary>
    public class DbSeeder
    {
        private readonly MongoDbContext _context;

        public DbSeeder(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Seeds all lookup data if collections are empty.
        /// </summary>
        public async Task SeedAsync()
        {
            await SeedPlayerPositionsAsync();
            await SeedMatchStatusesAsync();
            await SeedTeamStatusesAsync();
            await SeedSeasonStatusesAsync();
            await SeedMatchEventTypesAsync();
        }

        private async Task SeedPlayerPositionsAsync()
        {
            var count = await _context.PlayerPositions.CountDocumentsAsync(FilterDefinition<PlayerPosition>.Empty);
            if (count > 0) return;

            var positions = SeedData.PlayerPositions.DefaultPositions.Select(p => new PlayerPosition
            {
                Id = Guid.NewGuid().ToString(),
                Name = p.Name,
                Code = p.Code,
                Description = p.Description,
                DisplayOrder = p.Order,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.PlayerPositions.InsertManyAsync(positions);
        }

        private async Task SeedMatchStatusesAsync()
        {
            var count = await _context.MatchStatuses.CountDocumentsAsync(FilterDefinition<MatchStatus>.Empty);
            if (count > 0) return;

            var statuses = SeedData.MatchStatuses.DefaultStatuses.Select(s => new MatchStatus
            {
                Id = Guid.NewGuid().ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                ColorCode = s.ColorCode,
                DisplayOrder = s.Order,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.MatchStatuses.InsertManyAsync(statuses);
        }

        private async Task SeedTeamStatusesAsync()
        {
            var count = await _context.TeamStatuses.CountDocumentsAsync(FilterDefinition<TeamStatus>.Empty);
            if (count > 0) return;

            var statuses = SeedData.TeamStatuses.DefaultStatuses.Select(s => new TeamStatus
            {
                Id = Guid.NewGuid().ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                ColorCode = s.ColorCode,
                DisplayOrder = s.Order,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.TeamStatuses.InsertManyAsync(statuses);
        }

        private async Task SeedSeasonStatusesAsync()
        {
            var count = await _context.SeasonStatuses.CountDocumentsAsync(FilterDefinition<SeasonStatus>.Empty);
            if (count > 0) return;

            var statuses = SeedData.SeasonStatuses.DefaultStatuses.Select(s => new SeasonStatus
            {
                Id = Guid.NewGuid().ToString(),
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                ColorCode = s.ColorCode,
                DisplayOrder = s.Order,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.SeasonStatuses.InsertManyAsync(statuses);
        }

        private async Task SeedMatchEventTypesAsync()
        {
            var count = await _context.MatchEventTypes.CountDocumentsAsync(FilterDefinition<MatchEventType>.Empty);
            if (count > 0) return;

            var eventTypes = SeedData.MatchEventTypes.DefaultEventTypes.Select(e => new MatchEventType
            {
                Id = Guid.NewGuid().ToString(),
                Name = e.Name,
                Code = e.Code,
                Description = e.Description,
                IconName = e.Icon,
                ColorCode = e.ColorCode,
                DisplayOrder = e.Order,
                AffectsScore = e.AffectsScore,
                AffectsDiscipline = e.AffectsDiscipline,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            await _context.MatchEventTypes.InsertManyAsync(eventTypes);
        }
    }
}
