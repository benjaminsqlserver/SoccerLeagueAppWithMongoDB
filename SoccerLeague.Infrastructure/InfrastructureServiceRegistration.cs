using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;
using SoccerLeague.Infrastructure.Data;
using SoccerLeague.Infrastructure.Repositories;
using SoccerLeague.Infrastructure.Services;
using System.Text;

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
            services.AddScoped<IMatchStatusRepository, MatchStatusRepository>();
            services.AddScoped<IMatchEventTypeRepository, MatchEventTypeRepository>();
            services.AddScoped<IPlayerPositionRepository, PlayerPositionRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<ITeamStatusRepository, TeamStatusRepository>();
            services.AddScoped<ISeasonRepository, SeasonRepository>();

            services.AddScoped<ISeasonStatusRepository, SeasonStatusRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IStandingRepository, StandingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();

            // Register AuditLog repository
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

           
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            // Register authentication services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();


            // Register AuditLog services
            services.AddScoped<AuditLogService>();

            // Register HttpClient for GoogleAuthService
            services.AddHttpClient();

            // Register background service for cleanup 
            services.AddHostedService<AuditLogCleanupService>();
            services.AddHostedService<SessionCleanupService>();

            // Configure JWT Authentication
            var jwtSettings = configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var key = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


            // Register seeder
            services.AddScoped<DbSeeder>();

            return services;
        }
    }
}
