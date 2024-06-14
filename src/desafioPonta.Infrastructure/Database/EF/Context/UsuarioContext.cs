using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Infrastructure.Database.EF.Shared;
using Microsoft.EntityFrameworkCore;
namespace desafioPonta.Infrastructure.Database.EF.Context;

public class UsuarioContext : BaseContext<UsuarioEntity>
{
    public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options) { }
    
    public DbSet<UsuarioEntity> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UsuarioEntity>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();

        });

        base.OnModelCreating(builder);
    }

}

