using Demo.Repository.Pattern.Domain;
using Demo.Repository.Pattern.Specifications;
using System.Linq.Expressions;

namespace Demo.Repository.Pattern.Data
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<T?> GetAsync<TKey>(TKey id);

        Task<List<T>> GetAllAsync(int pageNumber, int pageSize);

        Task<List<T1>> FindAsync<T1>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<int> SaveChangesAsync();

        #region optional

        // Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task<List<T>> FindAsync(BaseSpecification<T> specification);

        // List<T> Find(Expression<Func<T, bool>> predicate);

        // List<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        #endregion
    }
}
