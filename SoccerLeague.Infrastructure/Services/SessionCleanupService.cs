using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Infrastructure.Services
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);

        public SessionCleanupService(
            IServiceProvider serviceProvider,
            ILogger<SessionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Session Cleanup Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

                    var result = await repository.CleanupExpiredSessionsAsync();
                    if (result)
                    {
                        _logger.LogInformation("Expired sessions cleaned up at {Time}", DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning up expired sessions");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Session Cleanup Service is stopping");
        }
    }
}
