using BNB.ProjetoReferencia.Core.Common.Helper;
using Microsoft.EntityFrameworkCore;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;

public abstract class BaseContext<TEntity> : DbContext
    where TEntity : Entity
{
    public BaseContext(DbContextOptions options)
        : base(options)
    {
        base.Database.EnsureCreated();
    }

    public DbSet<TEntity> Set { get; set; }
}
