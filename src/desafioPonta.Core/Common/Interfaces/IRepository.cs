using desafioPonta.Core.Common.Helper;

namespace desafioPonta.Core.Common.Interfaces;

public interface IReadOnlyRepository<TEntity, TKey>
       where TEntity : Entity<TKey>
{
    Task<TEntity> GetById(TKey id, CancellationToken ctx);
    Task<bool> AnyById(TKey id, CancellationToken ctx);
}

public interface IGetAllRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
{
    Task<List<TEntity>> GetAll(CancellationToken ctx);
}

public interface IRepository<TEntity, TKey>
        : IReadOnlyRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
{
    Task<TEntity> Add(TEntity entity, CancellationToken ctx);
    Task<TEntity> Update(TEntity entity, CancellationToken ctx);
}