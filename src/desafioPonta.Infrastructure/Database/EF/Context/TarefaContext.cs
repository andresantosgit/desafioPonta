using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Infrastructure.Database.EF.Shared;
using Microsoft.EntityFrameworkCore;

namespace desafioPonta.Infrastructure.Database.EF.Context;


public class TarefaContext : BaseContext<TarefaEntity>
{
    public TarefaContext(DbContextOptions<TarefaContext> options) : base(options) { }

    public DbSet<TarefaEntity> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TarefaEntity>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
        });

        base.OnModelCreating(builder);
    }

    
}
