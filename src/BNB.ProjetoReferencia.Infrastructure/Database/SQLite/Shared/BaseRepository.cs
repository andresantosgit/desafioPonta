using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;

public abstract class BaseRepository<TEntity, Tkey> : IBaseRepository<TEntity, Tkey>
    where TEntity : Entity<Tkey>
{
    protected readonly BaseContext<TEntity> _context;

    protected BaseRepository(BaseContext<TEntity> context)
    {
        _context = context;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ctx) => (await _context.AddAsync(entity, ctx)).Entity;

    public void Delete(TEntity entity) => _context.Remove(entity);

    public IEnumerable<TEntity> GetAll() => _context.Set.ToList();

    public TEntity GetById(Tkey id) => _context.Set.Find(id);

    public TEntity Update(TEntity entity) => _context.Set.Update(entity).Entity;

    public async Task SaveAsync(CancellationToken ctx) => await _context.SaveChangesAsync(ctx);
}
