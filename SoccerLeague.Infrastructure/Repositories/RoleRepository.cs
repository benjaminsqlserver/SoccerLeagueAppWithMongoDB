using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Infrastructure.Data;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Role entity operations.
    /// Provides specialized queries for roles including permission management.
    /// </summary>
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(MongoDbContext context)
            : base(context, "Roles")
        {
        }

        /// <summary>
        /// Gets a paginated list of roles with advanced filtering options.
        /// </summary>
        public async Task<PagedResult<Role>> GetRolesAsync(RoleQueryParameters parameters)
        {
            var filterBuilder = Builders<Role>.Filter;
            var filter = filterBuilder.Eq(r => r.IsDeleted, false);

            // Apply specific filters
            if (parameters.IsActive.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.IsActive, parameters.IsActive.Value);
            }

            if (parameters.IsSystemRole.HasValue)
            {
                filter &= filterBuilder.Eq(r => r.IsSystemRole, parameters.IsSystemRole.Value);
            }

            // Apply search term
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                filter &= BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildRoleSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get results
            var roles = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Role>
            {
                Items = roles,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        /// <summary>
        /// Gets a role by name.
        /// </summary>
        public async Task<Role?> GetByNameAsync(string name)
        {
            var normalizedName = name.ToUpper();
            var filter = Builders<Role>.Filter.Eq(r => r.NormalizedName, normalizedName) &
                        Builders<Role>.Filter.Eq(r => r.IsDeleted, false);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all active roles.
        /// </summary>
        public async Task<IReadOnlyList<Role>> GetActiveRolesAsync()
        {
            var filter = Builders<Role>.Filter.Eq(r => r.IsActive, true) &
                        Builders<Role>.Filter.Eq(r => r.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(r => r.DisplayOrder)
                .ThenBy(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all system roles.
        /// </summary>
        public async Task<IReadOnlyList<Role>> GetSystemRolesAsync()
        {
            var filter = Builders<Role>.Filter.Eq(r => r.IsSystemRole, true) &
                        Builders<Role>.Filter.Eq(r => r.IsDeleted, false);

            return await _collection.Find(filter)
                .SortBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if a role name already exists.
        /// </summary>
        public async Task<bool> RoleNameExistsAsync(string name, string? excludeId = null)
        {
            var normalizedName = name.ToUpper();
            var filterBuilder = Builders<Role>.Filter;
            var filter = filterBuilder.Eq(r => r.NormalizedName, normalizedName) &
                        filterBuilder.Eq(r => r.IsDeleted, false);

            if (!string.IsNullOrWhiteSpace(excludeId))
            {
                filter &= filterBuilder.Ne(r => r.Id, excludeId);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Checks if a role is a system role.
        /// </summary>
        public async Task<bool> IsSystemRoleAsync(string roleId)
        {
            var filter = Builders<Role>.Filter.Eq(r => r.Id, roleId) &
                        Builders<Role>.Filter.Eq(r => r.IsSystemRole, true);

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        /// <summary>
        /// Adds a permission to a role.
        /// </summary>
        public async Task<bool> AddPermissionToRoleAsync(string roleId, string permission)
        {
            var filter = Builders<Role>.Filter.Eq(r => r.Id, roleId);
            var update = Builders<Role>.Update
                .AddToSet(r => r.Permissions, permission)
                .Set(r => r.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        public async Task<bool> RemovePermissionFromRoleAsync(string roleId, string permission)
        {
            var filter = Builders<Role>.Filter.Eq(r => r.Id, roleId);
            var update = Builders<Role>.Update
                .Pull(r => r.Permissions, permission)
                .Set(r => r.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Updates all permissions for a role.
        /// </summary>
        public async Task<bool> UpdatePermissionsAsync(string roleId, List<string> permissions)
        {
            var filter = Builders<Role>.Filter.Eq(r => r.Id, roleId);
            var update = Builders<Role>.Update
                .Set(r => r.Permissions, permissions)
                .Set(r => r.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Builds search filter for name and description fields.
        /// </summary>
        protected override FilterDefinition<Role> BuildSearchFilter(string searchTerm)
        {
            var filter = Builders<Role>.Filter;
            return filter.Or(
                filter.Regex(r => r.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filter.Regex(r => r.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
        }

        /// <summary>
        /// Builds sort definition based on parameters.
        /// </summary>
        private SortDefinition<Role> BuildRoleSortDefinition(string? sortBy, bool sortDescending)
        {
            var sortBuilder = Builders<Role>.Sort;

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortBuilder.Ascending(r => r.DisplayOrder).Ascending(r => r.Name);
            }

            return sortBy.ToLower() switch
            {
                "name" => sortDescending
                    ? sortBuilder.Descending(r => r.Name)
                    : sortBuilder.Ascending(r => r.Name),
                "displayorder" => sortDescending
                    ? sortBuilder.Descending(r => r.DisplayOrder)
                    : sortBuilder.Ascending(r => r.DisplayOrder),
                "createddate" => sortDescending
                    ? sortBuilder.Descending(r => r.CreatedDate)
                    : sortBuilder.Ascending(r => r.CreatedDate),
                _ => sortBuilder.Ascending(r => r.DisplayOrder).Ascending(r => r.Name)
            };
        }
    }
}
