using MongoDB.Driver;
using Microsoft.Extensions.Options;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Infrastructure.Data
{
    /// <summary>
    /// MongoDB context for managing database connections and collections.
    /// Provides access to all collections in the SoccerLeague database.
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Collection properties for all entities
        public IMongoCollection<Match> Matches => _database.GetCollection<Match>("Matches");
        public IMongoCollection<MatchEventType> MatchEventTypes => _database.GetCollection<MatchEventType>("MatchEventTypes");
        public IMongoCollection<MatchStatus> MatchStatuses => _database.GetCollection<MatchStatus>("MatchStatuses");
        public IMongoCollection<Season> Seasons => _database.GetCollection<Season>("Seasons");
        public IMongoCollection<SeasonStatus> SeasonStatuses => _database.GetCollection<SeasonStatus>("SeasonStatuses");
        public IMongoCollection<Team> Teams => _database.GetCollection<Team>("Teams");
        public IMongoCollection<TeamStatus> TeamStatuses => _database.GetCollection<TeamStatus>("TeamStatuses");
        public IMongoCollection<Player> Players => _database.GetCollection<Player>("Players");
        public IMongoCollection<PlayerPosition> PlayerPositions => _database.GetCollection<PlayerPosition>("PlayerPositions");
        public IMongoCollection<Standing> Standings => _database.GetCollection<Standing>("Standings");

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

        /// <summary>
        /// Gets a collection by name for generic operations.
        /// </summary>
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

    /// <summary>
    /// Configuration settings for MongoDB connection.
    /// </summary>
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
