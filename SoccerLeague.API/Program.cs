namespace SoccerLeague.API
{
    using Microsoft.OpenApi.Models;
    using SoccerLeague.API.Middleware;
    using SoccerLeague.Application;
    using SoccerLeague.Infrastructure;
    using SoccerLeague.Infrastructure.Data;
    using System.Text.Json.Serialization;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Handle JSON serialization for enums and other types
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Soccer League API", Version = "v1" });

                // Add JWT authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });


            // Register Application Services (MediatR, AutoMapper, FluentValidation)
            builder.Services.AddApplicationServices();

            // Register Infrastructure Services (MongoDB, Repositories)
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var app = builder.Build();

            // Seed database on startup
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.SeedAsync();

                // Optionally create indexes
                var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                var indexer = new MongoDbIndexes(context);
                await indexer.CreateIndexesAsync();
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Soccer League API V1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            // Use global exception handler
            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");


            app.UseAuthentication();  // Must come before UseAuthorization
            app.UseAuthorization();

            app.UseAuditLogging();

            app.MapControllers();

            app.Run();
        }
    }
}
