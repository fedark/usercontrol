using System.Linq.Expressions;

namespace Data.Infrastructure.Abstractions;
public interface IUcSet<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task UpdateAsync(TEntity entity);
    Task RemoveAsync(TEntity entity);
}
