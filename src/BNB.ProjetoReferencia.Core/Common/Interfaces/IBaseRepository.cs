
namespace BNB.ProjetoReferencia.Core.Common.Interfaces;

public interface IBaseRepository<TEntity, TKey>
{
    TEntity GetById(TKey id);
    IEnumerable<TEntity> GetAll();
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ctx);
    TEntity Update(TEntity entity);
    void Delete(TEntity entity);
    Task SaveAsync(CancellationToken ctx);
}
