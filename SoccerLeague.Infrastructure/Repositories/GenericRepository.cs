using MongoDB.Driver;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Common;
using SoccerLeague.Infrastructure.Data;
using System.Linq.Expressions;

namespace SoccerLeague.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation for MongoDB operations.
    /// Provides common CRUD operations for all entities.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public GenericRepository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id) &
                         Builders<T>.Filter.Eq(e => e.IsDeleted, false);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            var filter = Builders<T>.Filter.Eq(e => e.IsDeleted, false);
            return await _collection.Find(filter).ToListAsync();
        }

        public virtual async Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters)
        {
            var filterBuilder = Builders<T>.Filter;
            var filter = filterBuilder.Eq(e => e.IsDeleted, false);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                // This is a basic implementation - override in specific repositories for proper search
                filter = filter & BuildSearchFilter(parameters.SearchTerm);
            }

            // Count total documents
            var totalCount = await _collection.CountDocumentsAsync(filter);

            // Build sort
            var sort = BuildSortDefinition(parameters.SortBy, parameters.SortDescending);

            // Get paginated results
            var items = await _collection.Find(filter)
                .Sort(sort)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Limit(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = (int)totalCount
            };
        }

        public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var filter = Builders<T>.Filter.Where(predicate) &
                         Builders<T>.Filter.Eq(e => e.IsDeleted, false);
            return await _collection.Find(filter).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            // Soft delete
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            var update = Builders<T>.Update
                .Set(e => e.IsDeleted, true)
                .Set(e => e.ModifiedDate, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public virtual async Task<bool> ExistsAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id) &
                         Builders<T>.Filter.Eq(e => e.IsDeleted, false);
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var filter = Builders<T>.Filter.Eq(e => e.IsDeleted, false);

            if (predicate != null)
            {
                filter = filter & Builders<T>.Filter.Where(predicate);
            }

            var count = await _collection.CountDocumentsAsync(filter);
            return (int)count;
        }

        /// <summary>
        /// Builds search filter - override in specific repositories for proper implementation
        /// </summary>
        protected virtual FilterDefinition<T> BuildSearchFilter(string searchTerm)
        {
            // Default implementation returns empty filter
            // Override in specific repositories to implement search logic
            return Builders<T>.Filter.Empty;
        }

        /// <summary>
        /// Builds sort definition based on parameters
        /// </summary>
        protected virtual SortDefinition<T> BuildSortDefinition(string? sortBy, bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Default sort by CreatedDate descending
                return Builders<T>.Sort.Descending(e => e.CreatedDate);
            }

            var sortBuilder = Builders<T>.Sort;
            return sortDescending
                ? sortBuilder.Descending(sortBy)
                : sortBuilder.Ascending(sortBy);
        }
    }
}
