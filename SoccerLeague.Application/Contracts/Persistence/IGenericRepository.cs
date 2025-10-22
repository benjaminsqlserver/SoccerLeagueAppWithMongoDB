using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Contracts.Persistence
{
    using SoccerLeague.Application.Common.Models;
    using System.Linq.Expressions;

    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}

