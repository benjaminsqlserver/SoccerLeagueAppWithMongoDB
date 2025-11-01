using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Infrastructure.Services
{
    /// <summary>
    /// Background service for automatically cleaning up old audit logs based on retention policy.
    /// </summary>
    public class AuditLogCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuditLogCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromDays(1); // Run daily
        private readonly int _retentionDays = 365; // Keep logs for 1 year

        public AuditLogCleanupService(IServiceProvider serviceProvider, ILogger<AuditLogCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditLog Cleanup Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

                    var cutoffDate = DateTime.UtcNow.AddDays(-_retentionDays);
                    _logger.LogInformation("Starting cleanup of audit logs older than {CutoffDate}", cutoffDate);

                    var result = await repository.DeleteOldAuditLogsAsync(cutoffDate);

                    if (result)
                    {
                        _logger.LogInformation("Successfully cleaned up old audit logs at {Time}", DateTime.UtcNow);
                    }
                    else
                    {
                        _logger.LogInformation("No old audit logs to clean up at {Time}", DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up old audit logs");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("AuditLog Cleanup Service is stopping");
        }
    }
}
