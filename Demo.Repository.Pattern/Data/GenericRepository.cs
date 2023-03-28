using Ardalis.Specification;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Demo.Repository.Pattern.Domain;
using Demo.Repository.Pattern.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Demo.Repository.Pattern.Data
{
    /*
     * - "Repository mediates between the domain and data mapping layer 
     * using a collection-like interface for accessing domain objects"
     * - "Repository also supports the objective of achieving a clean
     * seperation and one-way depedency between the domain and data mapping layer."
     * - The repository is an abstraction to reduce complexity and make the rest of
     * the code persistent ignorant.
     * - Helps us write unit tests (instead of integration tests)
     * - Generally repository should return domain objects
     * - We don't want to expose the data access layer (DAL)
     * - We want to prevent 
     *      - Arbitrary db queries in code
     *      - Leaking db concerns into app logig
     *      - App coupling with EF (not ORM agnostic) 
     * - 
     */
    public class GenericRepository<TEntity, TContext> : IRepository<TEntity>, IDisposable
        where TEntity : BaseEntity
        where TContext : DbContext
    {
        protected readonly DbSet<TEntity> DbSet;

        protected readonly TContext Context;

        protected readonly IMapper mapper;

        private bool disposed;

        public GenericRepository(TContext context, IMapper mapper)
        {
            this.Context = context;
            this.DbSet = this.Context.Set<TEntity>();
            this.mapper = mapper;
        }

        public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.DbSet.Where(predicate).AsNoTracking().ToListAsync();
        }

        public virtual async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = this.GetAllIncluding(includeProperties);
            List<TEntity> results = await query.Where(predicate).ToListAsync();
            return results;
        }

        public virtual async Task<List<T1>> FindAsync<T1>(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            // TODO: Add pagination (skip/take) support

            IQueryable<TEntity> query = this.GetAllIncluding(includeProperties);
            return await query.Where(predicate).ProjectTo<T1>(this.mapper.ConfigurationProvider).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber > 0 ? pageNumber - 1 : 1;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            return await this.DbSet.AsNoTracking().Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<TEntity?> GetAsync<TKey>(TKey id)
        {
            return await this.DbSet.FindAsync(id);
        }

        public virtual async Task Add(TEntity entity)
        {
            _ = await this.DbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            /*
             * TODO:
             * 
             * Using "Update" method on DbContext will mark all the fields as modified 
             * and will include all of them in the query.
             * 
             * To update a subset of fields use the "Attach" method and then mark the 
             * desired field as "Modified" or ".Property(x => x.Field).IsModified = true"
             */

            // AVOID using Update
            // this.DbSet.Update(entity);

            // Are we tracking the entity or not?
            TEntity? existingEntity = this.DbSet.Find(entity.Id);

            if (existingEntity != null)
            {
                EntityEntry<TEntity> existingEntityEntry = this.DbSet.Entry(existingEntity);
                existingEntityEntry.CurrentValues.SetValues(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            _ = this.DbSet.Remove(entity);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await this.Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }

            this.disposed = true;
        }

        private IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = this.DbSet.AsNoTracking();
            return includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual async Task<List<TEntity>> FindAsync(BaseSpecification<TEntity> specification)
        {
            ArgumentNullException.ThrowIfNull(specification, nameof(specification));
            return await this.DbSet.Where(specification.ToExpression()).ToListAsync();
        }
    }
}
