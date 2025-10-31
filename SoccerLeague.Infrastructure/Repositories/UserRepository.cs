using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for User entity operations.
    /// Provides specialized queries for users including authentication, role management, and account management.
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MongoDbContext context)
            : base(context, "Users")
        {
        }

        /// <summary>
        /// Gets a paginated list of users with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<User>> GetUsersAsync(UserQueryParameters parameters)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq(u => u.IsDeleted, false);

            // Apply specific filters
            if (parameters.EmailConfirmed.HasValue)
            {
                filter &= filterBuilder.Eq(u => u.EmailConfirmed, parameters.EmailConfirmed.Value);
            }

            if (parameters.IsActive.HasValue)
            {
                filter &= filterBuilder.Eq(u => u.IsActive, parameters.IsActive.Value);
            }

            if (parameters.IsLockedOut.HasValue)
            {
                if (parameters.IsLockedOut.Value)
                {
                    filter &= filterBuilder.Eq(u => u.LockoutEnabled, true) &
                             filterBuilder.Gt(u => u.LockoutEnd, DateTime.UtcNow);
                }
                else
                {
                    filter &= filterBuilder.Or(
                        filterBuilder.Eq(u => u.LockoutEnabled, false),
                        filterBuilder.Or(
                            filterBuilder.Eq(u => u.LockoutEnd, null),
                            filterBuilder.Lte(u => u.LockoutEnd, DateTime.UtcNow)
                        )
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.AuthProvider))
            {
                if (Enum.TryParse<AuthenticationProvider>(parameters.AuthProvider, true, out var authProvider))
                {
                    filter &= filterBuilder.Eq(u => u.AuthProvider, authProvider);
                }
            }

            if (parameters.LastLoginFrom.HasValue)
            {
                filter &= filterBuilder.Gte(u => u.LastLoginDate, parameters.LastLoginFrom.Value);
            }

            if (parameters.LastLoginTo.HasValue)
            {
                filter &= filterBuilder.Lte(u => u.LastLoginDate, parameters.LastLoginTo.Value);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildUserSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var users = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Items = users,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets a user by email address.
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email.ToLower()) &
                        Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a user by Google ID.
        /// </summary>
        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.GoogleId, googleId) &
                        Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all active users.
        /// </summary>
        public async Task<IReadOnlyList<User>> GetActiveUsersAsync()
        {
            var filter = Builders<User>.Filter.Eq(u => u.IsActive, true) &
                        Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all locked out users.
        /// </summary>
        public async Task<IReadOnlyList<User>> GetLockedOutUsersAsync()
        {
            var filter = Builders<User>.Filter.Eq(u => u.LockoutEnabled, true) &
                        Builders<User>.Filter.Gt(u => u.LockoutEnd, DateTime.UtcNow) &
                        Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return await _collection.Find(filter)
                .SortByDescending(u => u.LockoutEnd)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all users with a specific role.
        /// </summary>
        public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleId)
        {
            var filter = Builders<User>.Filter.AnyEq(u => u.RoleIds, roleId) &
                        Builders<User>.Filter.Eq(u => u.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if an email already exists.
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email, string? excludeId = null)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq(u => u.Email, email.ToLower()) &
                        filterBuilder.Eq(u => u.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(u => u.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Updates a user's password hash.
        /// </summary>
        public async Task<bool> UpdatePasswordAsync(string userId, string passwordHash)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, passwordHash)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Updates the last login date for a user.
        /// </summary>
        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.LastLoginDate, DateTime.UtcNow)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Increments the access failed count for a user.
        /// </summary>
        public async Task<bool> IncrementAccessFailedCountAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Inc(u => u.AccessFailedCount, 1)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Resets the access failed count to zero.
        /// </summary>
        public async Task<bool> ResetAccessFailedCountAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.AccessFailedCount, 0)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Locks out a user until a specified date.
        /// </summary>
        public async Task<bool> LockoutUserAsync(string userId, DateTime lockoutEnd)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.LockoutEnabled, true)
                .Set(u => u.LockoutEnd, lockoutEnd)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Removes lockout from a user.
        /// </summary>
        public async Task<bool> UnlockUserAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.LockoutEnabled, false)
                .Set(u => u.LockoutEnd, (DateTime?)null)
                .Set(u => u.AccessFailedCount, 0)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Marks a user's email as confirmed.
        /// </summary>
        public async Task<bool> ConfirmEmailAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update
                .Set(u => u.EmailConfirmed, true)
                .Set(u => u.EmailVerificationToken, (string?)null)
                .Set(u => u.EmailVerificationTokenExpiry, (DateTime?)null)
                .Set(u => u.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Builds search filter for email, first name, and last name fields.
        /// </summary>
        protected override FilterDefinition<User> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<User>.Filter;
            return filter.Or(
                filter.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(u => u.FirstName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(u => u.LastName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<User> BuildUserSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<User>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Ascending(u => u.LastName).Ascending(u => u.FirstName);
            }

            return sortBy.ToLower() switch
            {
                "email" => sortDescending
                    ? sortBuilder.Descending(u => u.Email)
                    : sortBuilder.Ascending(u => u.Email),
                "firstname" => sortDescending
                    ? sortBuilder.Descending(u => u.FirstName)
                    : sortBuilder.Ascending(u => u.FirstName),
                "lastname" => sortDescending
                    ? sortBuilder.Descending(u => u.LastName)
                    : sortBuilder.Ascending(u => u.LastName),
                "createddate" => sortDescending
                    ? sortBuilder.Descending(u => u.CreatedDate)
                    : sortBuilder.Ascending(u => u.CreatedDate),
                "lastlogindate" => sortDescending
                    ? sortBuilder.Descending(u => u.LastLoginDate)
                    : sortBuilder.Ascending(u => u.LastLoginDate),
                _ => sortBuilder.Ascending(u => u.LastName).Ascending(u => u.FirstName)
            };
        }
    }
}
