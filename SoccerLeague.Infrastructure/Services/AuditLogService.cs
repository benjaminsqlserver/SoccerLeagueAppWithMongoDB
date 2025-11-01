using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;

namespace SoccerLeague.Infrastructure.Services
{
    /// <summary>
    /// Helper service for creating audit log entries throughout the application.
    /// </summary>
    public class AuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Logs an action to the audit trail.
        /// </summary>
        public async Task<bool> LogActionAsync(
            string? userId,
            string? username,
            AuditActionType actionType,
            string entityType,
            string? entityId,
            string? entityName,
            string description,
            string? oldValues = null,
            string? newValues = null,
            string? ipAddress = null,
            string? userAgent = null,
            bool success = true,
            string? errorMessage = null,
            string? additionalData = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Username = username,
                    ActionType = actionType,
                    EntityType = entityType,
                    EntityId = entityId,
                    EntityName = entityName,
                    Description = description,
                    OldValues = oldValues,
                    NewValues = newValues,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    ActionDate = DateTime.UtcNow,
                    Success = success,
                    ErrorMessage = errorMessage,
                    AdditionalData = additionalData,
                    CreatedDate = DateTime.UtcNow
                };

                await _auditLogRepository.AddAsync(auditLog);
                return true;
            }
            catch
            {
                // Don't throw exceptions from audit logging to avoid disrupting main operations
                return false;
            }
        }

        /// <summary>
        /// Logs a user login.
        /// </summary>
        public async Task LogLoginAsync(string userId, string username, string? ipAddress, string? userAgent, bool success, string? errorMessage = null)
        {
            await LogActionAsync(
                userId: userId,
                username: username,
                actionType: success ? AuditActionType.Login : AuditActionType.LoginFailed,
                entityType: "User",
                entityId: userId,
                entityName: username,
                description: success ? "User logged in successfully" : $"Failed login attempt: {errorMessage}",
                ipAddress: ipAddress,
                userAgent: userAgent,
                success: success,
                errorMessage: errorMessage
            );
        }

        /// <summary>
        /// Logs a user logout.
        /// </summary>
        public async Task LogLogoutAsync(string userId, string username, string? ipAddress, string? userAgent)
        {
            await LogActionAsync(
                userId: userId,
                username: username,
                actionType: AuditActionType.Logout,
                entityType: "User",
                entityId: userId,
                entityName: username,
                description: "User logged out",
                ipAddress: ipAddress,
                userAgent: userAgent
            );
        }

        /// <summary>
        /// Logs a create operation.
        /// </summary>
        public async Task LogCreateAsync(string userId, string username, string entityType, string entityId, string entityName, string? newValues, string? ipAddress = null)
        {
            await LogActionAsync(
                userId: userId,
                username: username,
                actionType: AuditActionType.Create,
                entityType: entityType,
                entityId: entityId,
                entityName: entityName,
                description: $"Created {entityType}: {entityName}",
                newValues: newValues,
                ipAddress: ipAddress
            );
        }

        /// <summary>
        /// Logs an update operation.
        /// </summary>
        public async Task LogUpdateAsync(string userId, string username, string entityType, string entityId, string entityName, string? oldValues, string? newValues, string? ipAddress = null)
        {
            await LogActionAsync(
                userId: userId,
                username: username,
                actionType: AuditActionType.Update,
                entityType: entityType,
                entityId: entityId,
                entityName: entityName,
                description: $"Updated {entityType}: {entityName}",
                oldValues: oldValues,
                newValues: newValues,
                ipAddress: ipAddress
            );
        }

        /// <summary>
        /// Logs a delete operation.
        /// </summary>
        public async Task LogDeleteAsync(string userId, string username, string entityType, string entityId, string entityName, string? oldValues, string? ipAddress = null)
        {
            await LogActionAsync(
                userId: userId,
                username: username,
                actionType: AuditActionType.Delete,
                entityType: entityType,
                entityId: entityId,
                entityName: entityName,
                description: $"Deleted {entityType}: {entityName}",
                oldValues: oldValues,
                ipAddress: ipAddress
            );
        }
    }
}
