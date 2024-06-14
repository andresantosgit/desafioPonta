
namespace desafioPonta.Core.Common.Interfaces;

public interface IBaseRepository<TEntity, TKey> : IBaseRepository<TEntity>
{
    TEntity GetById(TKey id);
}

public interface IBaseRepository<TEntity>
{
    IEnumerable<TEntity> GetAll();
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ctx);
    TEntity Update(TEntity entity);
    void Delete(TEntity entity);
    Task SaveAsync(CancellationToken ctx);
}
