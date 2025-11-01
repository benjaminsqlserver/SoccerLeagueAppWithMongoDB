using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserSession entity operations.
    /// Provides specialized queries for session management, activity tracking, and cleanup.
    /// </summary>
    public class UserSessionRepository : GenericRepository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(MongoDbContext context)
            : base(context, "UserSessions")
        {
        }

        /// <summary>
        /// Gets a paginated list of sessions with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<UserSession>> GetSessionsAsync(UserSessionQueryParameters parameters)
        {
            var filterBuilder = Builders<UserSession>.Filter;
            var filter = filterBuilder.Eq(s => s.IsDeleted, false);

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(parameters.UserId))
            {
                filter &= filterBuilder.Eq(s => s.UserId, parameters.UserId);
            }

            if (parameters.IsActive.HasValue)
            {
                filter &= filterBuilder.Eq(s => s.IsActive, parameters.IsActive.Value);
            }

            if (parameters.SessionStartFrom.HasValue)
            {
                filter &= filterBuilder.Gte(s => s.SessionStartDate, parameters.SessionStartFrom.Value);
            }

            if (parameters.SessionStartTo.HasValue)
            {
                filter &= filterBuilder.Lte(s => s.SessionStartDate, parameters.SessionStartTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(parameters.DeviceType))
            {
                filter &= filterBuilder.Eq(s => s.DeviceType, parameters.DeviceType);
            }

            if (!string.IsNullOrWhiteSpace(parameters.IpAddress))
            {
                filter &= filterBuilder.Eq(s => s.IpAddress, parameters.IpAddress);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildSessionSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var sessions = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<UserSession>
            {
                Items = sessions,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets a session by refresh token.
        /// </summary>
        public async Task<UserSession?> GetByRefreshTokenAsync(string refreshToken)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.RefreshToken, refreshToken) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a session by token ID.
        /// </summary>
        public async Task<UserSession?> GetByTokenIdAsync(string tokenId)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.TokenId, tokenId) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all active sessions for a specific user.
        /// </summary>
        public async Task<IReadOnlyList<UserSession>> GetActiveSessionsByUserAsync(string userId)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.UserId, userId) &
                        Builders<UserSession>.Filter.Eq(s => s.IsActive, true) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(s => s.LastActivityDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all sessions (active and terminated) for a specific user.
        /// </summary>
        public async Task<IReadOnlyList<UserSession>> GetAllSessionsByUserAsync(string userId)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.UserId, userId) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(s => s.SessionStartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Terminates a specific session.
        /// </summary>
        public async Task<bool> TerminateSessionAsync(string sessionId, string terminationReason)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<UserSession>.Update
                .Set(s => s.IsActive, false)
                .Set(s => s.TerminationReason, terminationReason)
                .Set(s => s.TerminationDate, DateTime.UtcNow)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Terminates all active sessions for a user.
        /// </summary>
        public async Task<bool> TerminateAllUserSessionsAsync(string userId, string terminationReason)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.UserId, userId) &
                        Builders<UserSession>.Filter.Eq(s => s.IsActive, true);

            var update = Builders<UserSession>.Update
                .Set(s => s.IsActive, false)
                .Set(s => s.TerminationReason, terminationReason)
                .Set(s => s.TerminationDate, DateTime.UtcNow)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Updates the last activity date for a session.
        /// </summary>
        public async Task<bool> UpdateLastActivityAsync(string sessionId)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<UserSession>.Update
                .Set(s => s.LastActivityDate, DateTime.UtcNow)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Checks if a refresh token exists.
        /// </summary>
        public async Task<bool> RefreshTokenExistsAsync(string refreshToken)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.RefreshToken, refreshToken) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Gets the count of active sessions for a user.
        /// </summary>
        public async Task<int> GetActiveSessionCountByUserAsync(string userId)
        {
            var filter = Builders<UserSession>.Filter.Eq(s => s.UserId, userId) &
                        Builders<UserSession>.Filter.Eq(s => s.IsActive, true) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            var count = await _collection.CountDocumentsAsync(filter);
            return (int)count;
        }

        /// <summary>
        /// Cleans up expired sessions by marking them as inactive.
        /// </summary>
        public async Task<bool> CleanupExpiredSessionsAsync()
        {
            var filter = Builders<UserSession>.Filter.Lt(s => s.SessionExpiryDate, DateTime.UtcNow) &
                        Builders<UserSession>.Filter.Eq(s => s.IsActive, true) &
                        Builders<UserSession>.Filter.Eq(s => s.IsDeleted, false);

            var update = Builders<UserSession>.Update
                .Set(s => s.IsActive, false)
                .Set(s => s.TerminationReason, "Session Expired")
                .Set(s => s.TerminationDate, DateTime.UtcNow)
                .Set(s => s.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Builds search filter for IP address, user agent, device type, and location fields.
        /// </summary>
        protected override FilterDefinition<UserSession> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<UserSession>.Filter;
            return filter.Or(
                filter.Regex(s => s.IpAddress, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.UserAgent, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.DeviceType, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.DeviceName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.BrowserName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.City, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(s => s.Country, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<UserSession> BuildSessionSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<UserSession>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Descending(s => s.SessionStartDate);
            }

            return sortBy.ToLower() switch
            {
                "sessionstartdate" => sortDescending
                    ? sortBuilder.Descending(s => s.SessionStartDate)
                    : sortBuilder.Ascending(s => s.SessionStartDate),
                "sessionexpirydate" => sortDescending
                    ? sortBuilder.Descending(s => s.SessionExpiryDate)
                    : sortBuilder.Ascending(s => s.SessionExpiryDate),
                "lastactivitydate" => sortDescending
                    ? sortBuilder.Descending(s => s.LastActivityDate)
                    : sortBuilder.Ascending(s => s.LastActivityDate),
                "isactive" => sortDescending
                    ? sortBuilder.Descending(s => s.IsActive)
                    : sortBuilder.Ascending(s => s.IsActive),
                "devicetype" => sortDescending
                    ? sortBuilder.Descending(s => s.DeviceType)
                    : sortBuilder.Ascending(s => s.DeviceType),
                "ipaddress" => sortDescending
                    ? sortBuilder.Descending(s => s.IpAddress)
                    : sortBuilder.Ascending(s => s.IpAddress),
                _ => sortBuilder.Descending(s => s.SessionStartDate)
            };
        }
    }
}
