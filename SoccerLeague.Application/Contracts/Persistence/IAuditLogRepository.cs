using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Contracts.Persistence
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<PagedResult<AuditLog>> GetAuditLogsAsync(AuditLogQueryParameters parameters);
        Task<IReadOnlyList<AuditLog>> GetAuditLogsByUserAsync(string userId, int limit = 100);
        Task<IReadOnlyList<AuditLog>> GetAuditLogsByEntityAsync(string entityType, string entityId);
        Task<IReadOnlyList<AuditLog>> GetAuditLogsByActionTypeAsync(AuditActionType actionType, int limit = 100);
        Task<IReadOnlyList<AuditLog>> GetFailedActionsAsync(int limit = 100);
        Task<IReadOnlyList<AuditLog>> GetRecentActivityAsync(int limit = 50);
        Task<IReadOnlyList<AuditLog>> GetSecurityEventsAsync(int limit = 100);
        Task<int> GetAuditLogCountByUserAsync(string userId, DateTime? from = null, DateTime? to = null);
        Task<int> GetAuditLogCountByActionTypeAsync(AuditActionType actionType, DateTime? from = null, DateTime? to = null);
        Task<bool> DeleteOldAuditLogsAsync(DateTime olderThan);
    }
}
