using System.Linq.Expressions;

namespace Data.Infrastructure.Abstractions;
public interface IDataSet<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
