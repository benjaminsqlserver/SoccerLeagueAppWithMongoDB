using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for AuditLog entity operations.
    /// Provides specialized queries for audit trails, security monitoring, and compliance reporting.
    /// </summary>
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(MongoDbContext context)
            : base(context, "AuditLogs")
        {
        }

        /// <summary>
        /// Gets a paginated list of audit logs with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<AuditLog>> GetAuditLogsAsync(AuditLogQueryParameters parameters)
        {
            var filterBuilder = Builders<AuditLog>.Filter;
            var filter = filterBuilder.Eq(a => a.IsDeleted, false);

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(parameters.UserId))
            {
                filter &= filterBuilder.Eq(a => a.UserId, parameters.UserId);
            }

            if (!string.IsNullOrWhiteSpace(parameters.EntityType))
            {
                filter &= filterBuilder.Eq(a => a.EntityType, parameters.EntityType);
            }

            if (!string.IsNullOrWhiteSpace(parameters.EntityId))
            {
                filter &= filterBuilder.Eq(a => a.EntityId, parameters.EntityId);
            }

            if (parameters.ActionType.HasValue)
            {
                filter &= filterBuilder.Eq(a => a.ActionType, parameters.ActionType.Value);
            }

            if (parameters.Success.HasValue)
            {
                filter &= filterBuilder.Eq(a => a.Success, parameters.Success.Value);
            }

            if (parameters.ActionDateFrom.HasValue)
            {
                filter &= filterBuilder.Gte(a => a.ActionDate, parameters.ActionDateFrom.Value);
            }

            if (parameters.ActionDateTo.HasValue)
            {
                filter &= filterBuilder.Lte(a => a.ActionDate, parameters.ActionDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(parameters.IpAddress))
            {
                filter &= filterBuilder.Eq(a => a.IpAddress, parameters.IpAddress);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildAuditLogSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var auditLogs = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<AuditLog>
            {
                Items = auditLogs,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets audit logs for a specific user.
        /// </summary>
        public async Task<IReadOnlyList<AuditLog>> GetAuditLogsByUserAsync(string userId, int limit = 100)
        {
            var filter = Builders<AuditLog>.Filter.Eq(a => a.UserId, userId) &
                        Builders<AuditLog>.Filter.Eq(a => a.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(a => a.ActionDate)
                .Limit(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all audit logs for a specific entity.
        /// </summary>
        public async Task<IReadOnlyList<AuditLog>> GetAuditLogsByEntityAsync(string entityType, string entityId)
        {
            var filter = Builders<AuditLog>.Filter.Eq(a => a.EntityType, entityType) &
                        Builders<AuditLog>.Filter.Eq(a => a.EntityId, entityId) &
                        Builders<AuditLog>.Filter.Eq(a => a.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(a => a.ActionDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets audit logs by action type.
        /// </summary>
        public async Task<IReadOnlyList<AuditLog>> GetAuditLogsByActionTypeAsync(AuditActionType actionType, int limit = 100)
        {
            var filter = Builders<AuditLog>.Filter.Eq(a => a.ActionType, actionType) &
                        Builders<AuditLog>.Filter.Eq(a => a.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(a => a.ActionDate)
                .Limit(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Gets failed actions for troubleshooting.
        /// </summary>
        public async Task<IReadOnlyList<AuditLog>> GetFailedActionsAsync(int limit = 100)
        {
            var filter = Builders<AuditLog>.Filter.Eq(a => a.Success, false) &
                        Builders<AuditLog>.Filter.Eq(a => a.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(a => a.ActionDate)
                .Limit(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recent activity across the system.
        /// </summary>
        public async Task<IReadOnlyList<AuditLog>> GetRecentActivityAsync(int limit = 50)
        {
            var filter = Builders<AuditLog>.Filter.Eq(a => a.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(a => a.ActionDate)
                .Limit(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Gets security-related events (login, logout, password changes, lockouts).
        /// </summary>
        public async Task<IReadOnlyList<AuditLog>> GetSecurityEventsAsync(int limit = 100)
        {
            var securityActionTypes = new[]
            {
                AuditActionType.Login,
                AuditActionType.Logout,
                AuditActionType.LoginFailed,
                AuditActionType.PasswordChanged,
                AuditActionType.PasswordResetRequested,
                AuditActionType.EmailConfirmed,
                AuditActionType.UserLockedOut,
                AuditActionType.UserUnlocked
            };

            var filter = Builders<AuditLog>.Filter.In(a => a.ActionType, securityActionTypes) &
                        Builders<AuditLog>.Filter.Eq(a => a.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(a => a.ActionDate)
                .Limit(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the count of audit logs for a user within a date range.
        /// </summary>
        public async Task<int> GetAuditLogCountByUserAsync(string userId, DateTime? from = null, DateTime? to = null)
        {
            var filterBuilder = Builders<AuditLog>.Filter;
            var filter = filterBuilder.Eq(a => a.UserId, userId) &
                        filterBuilder.Eq(a => a.IsDeleted, false);

            if (from.HasValue)
            {
                filter &= filterBuilder.Gte(a => a.ActionDate, from.Value);
            }

            if (to.HasValue)
            {
                filter &= filterBuilder.Lte(a => a.ActionDate, to.Value);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return (int)count;
        }

        /// <summary>
        /// Gets the count of audit logs by action type within a date range.
        /// </summary>
        public async Task<int> GetAuditLogCountByActionTypeAsync(AuditActionType actionType, DateTime? from = null, DateTime? to = null)
        {
            var filterBuilder = Builders<AuditLog>.Filter;
            var filter = filterBuilder.Eq(a => a.ActionType, actionType) &
                        filterBuilder.Eq(a => a.IsDeleted, false);

            if (from.HasValue)
            {
                filter &= filterBuilder.Gte(a => a.ActionDate, from.Value);
            }

            if (to.HasValue)
            {
                filter &= filterBuilder.Lte(a => a.ActionDate, to.Value);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return (int)count;
        }

        /// <summary>
        /// Deletes audit logs older than a specified date (for data retention compliance).
        /// </summary>
        public async Task<bool> DeleteOldAuditLogsAsync(DateTime olderThan)
        {
            var filter = Builders<AuditLog>.Filter.Lt(a => a.ActionDate, olderThan);

            var result = await _collection.DeleteManyAsync(filter);

            return result.DeletedCount > 0;
        }

        /// <summary>
        /// Builds search filter for username, description, entity name, and entity type fields.
        /// </summary>
        protected override FilterDefinition<AuditLog> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<AuditLog>.Filter;
            return filter.Or(
                filter.Regex(a => a.Username, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(a => a.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(a => a.EntityName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(a => a.EntityType, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<AuditLog> BuildAuditLogSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<AuditLog>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Descending(a => a.ActionDate);
            }

            return sortBy.ToLower() switch
            {
                "actiondate" => sortDescending
                    ? sortBuilder.Descending(a => a.ActionDate)
                    : sortBuilder.Ascending(a => a.ActionDate),
                "actiontype" => sortDescending
                    ? sortBuilder.Descending(a => a.ActionType)
                    : sortBuilder.Ascending(a => a.ActionType),
                "entitytype" => sortDescending
                    ? sortBuilder.Descending(a => a.EntityType)
                    : sortBuilder.Ascending(a => a.EntityType),
                "username" => sortDescending
                    ? sortBuilder.Descending(a => a.Username)
                    : sortBuilder.Ascending(a => a.Username),
                "success" => sortDescending
                    ? sortBuilder.Descending(a => a.Success)
                    : sortBuilder.Ascending(a => a.Success),
                _ => sortBuilder.Descending(a => a.ActionDate)
            };
        }
    }
}
