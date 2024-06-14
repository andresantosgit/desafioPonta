using desafioPonta.Core.Common.Helper;
using Microsoft.EntityFrameworkCore;

namespace desafioPonta.Infrastructure.Database.EF.Shared;

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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   