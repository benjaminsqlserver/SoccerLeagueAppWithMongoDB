using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Infrastructure.Data;
using SoccerLeague.Infrastructure.Repositories;

namespace SoccerLeague.Infrastructure
{
    /// <summary>
    /// Extension methods for registering infrastructure services.
    /// </summary>
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register MongoDB settings
            services.Configure<MongoDbSettings>(
                configuration.GetSection("MongoDbSettings"));

            // Register MongoDB context
            services.AddSingleton<MongoDbContext>();

            // Register repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IMatchStatusRepository, MatchStatusRepository>();
            services.AddScoped<IMatchEventTypeRepository, MatchEventTypeRepository>();
            services.AddScoped<IPlayerPositionRepository, PlayerPositionRepository>();
            services.AddScoped<ITeamStatusRepository, TeamStatusRepository>();

            // Register seeder
            services.AddScoped<DbSeeder>();

            return services;
        }
    }
}
