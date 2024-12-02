using System.Linq.Expressions;
using CcStore.Models;

namespace CcStore.Repository.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task CreateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> FindAllAsync();
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> FindByIdAsync(object id);
        Task UpdateAsync(T entity);
    }
}