using desafioPonta.Core.Common.Helper;
using desafioPonta.Core.Common.Interfaces;

namespace desafioPonta.Infrastructure.Database.EF.Shared;

public abstract class BaseRepository<TEntity, Tkey> : BaseRepository<TEntity>, IBaseRepository<TEntity, Tkey>
    where TEntity : Entity<Tkey>
{
    protected BaseRepository(BaseContext<TEntity> context) : base(context)
    {

    }
    public TEntity GetById(Tkey id) => _context.Set.Find(id);

}

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : Entity
{
    protected readonly BaseContext<TEntity> _context;

    protected BaseRepository(BaseContext<TEntity> context)
    {
        _context = context;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ctx) => (await _context.AddAsync(entity, ctx)).Entity;

    public void Delete(TEntity entity) => _context.Remove(entity);

    public IEnumerable<TEntity> GetAll() => _context.Set.ToList();

    public TEntity Update(TEntity entity) => _context.Set.Update(entity).Entity;

    public async Task SaveAsync(CancellationToken ctx) => await _context.SaveChangesAsync(ctx);
}
