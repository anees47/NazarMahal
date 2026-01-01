using System.Linq.Expressions;

namespace NazarMahal.Application.Interfaces.IRepository
{
    public interface IRepository<in TKey, TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> GetAsync(TKey id);

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includes
        );

        Task CompleteAsync();
    }
}
