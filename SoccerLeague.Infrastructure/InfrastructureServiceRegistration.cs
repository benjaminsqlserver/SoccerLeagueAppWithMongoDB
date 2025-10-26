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
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMatchEventTypeRepository, MatchEventTypeRepository>();

            // Register seeder
            services.AddScoped<DbSeeder>();

            return services;
        }
    }
}
