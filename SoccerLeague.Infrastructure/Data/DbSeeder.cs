using MongoDB.Bson;
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
             await SeedRolesAsync();
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


        private async Task SeedRolesAsync()
        {
            var count = await _context.Roles.CountDocumentsAsync(FilterDefinition<Role>.Empty);
            if (count > 0) return;

            var roles = new List<Role>
    {
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.Administrator,
            NormalizedName = SystemRoles.Administrator.ToUpper(),
            Description = "Full system access with all permissions",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 1,
            Permissions = Permissions.AllPermissions.ToList(),
            CreatedDate = DateTime.UtcNow
        },
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.Manager,
            NormalizedName = SystemRoles.Manager.ToUpper(),
            Description = "League operations management access",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 2,
            Permissions = new List<string>
            {
                Permissions.ViewMatches, Permissions.CreateMatches,
                Permissions.UpdateMatches, Permissions.ManageMatchEvents,
                Permissions.ViewTeams, Permissions.ViewPlayers,
                Permissions.ViewSeasons, Permissions.ViewStandings,
                Permissions.ViewReports
            },
            CreatedDate = DateTime.UtcNow
        },
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.Referee,
            NormalizedName = SystemRoles.Referee.ToUpper(),
            Description = "Match management and event recording",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 3,
            Permissions = new List<string>
            {
                Permissions.ViewMatches, Permissions.UpdateMatches,
                Permissions.ManageMatchEvents
            },
            CreatedDate = DateTime.UtcNow
        },
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.TeamManager,
            NormalizedName = SystemRoles.TeamManager.ToUpper(),
            Description = "Team and player management",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 4,
            Permissions = new List<string>
            {
                Permissions.ViewTeams, Permissions.UpdateTeams,
                Permissions.ViewPlayers, Permissions.CreatePlayers,
                Permissions.UpdatePlayers, Permissions.ViewMatches,
                Permissions.ViewStandings
            },
            CreatedDate = DateTime.UtcNow
        },
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.Player,
            NormalizedName = SystemRoles.Player.ToUpper(),
            Description = "Player profile access",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 5,
            Permissions = new List<string>
            {
                Permissions.ViewMatches, Permissions.ViewTeams,
                Permissions.ViewPlayers, Permissions.ViewStandings
            },
            CreatedDate = DateTime.UtcNow
        },
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.Fan,
            NormalizedName = SystemRoles.Fan.ToUpper(),
            Description = "View-only access to matches and standings",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 6,
            Permissions = new List<string>
            {
                Permissions.ViewMatches, Permissions.ViewTeams,
                Permissions.ViewPlayers, Permissions.ViewStandings
            },
            CreatedDate = DateTime.UtcNow
        },
        new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = SystemRoles.User,
            NormalizedName = SystemRoles.User.ToUpper(),
            Description = "Basic registered user access",
            IsSystemRole = true,
            IsActive = true,
            DisplayOrder = 7,
            Permissions = new List<string>
            {
                Permissions.ViewMatches, Permissions.ViewStandings
            },
            CreatedDate = DateTime.UtcNow
        }
    };

            await _context.Roles.InsertManyAsync(roles);
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
