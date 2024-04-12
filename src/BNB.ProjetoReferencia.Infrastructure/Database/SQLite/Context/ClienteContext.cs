using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;
using Microsoft.EntityFrameworkCore;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;

public class ClienteContext : BaseContext<ClienteEntity>
{
    public ClienteContext(DbContextOptions<ClienteContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ClienteEntity>(entity =>
        {
            entity.ToTable("sub_cliente");

            // Indica que é a chave primária
            entity.HasKey(m => m.Id);

            // Configura o Id para ser autoincremento
            entity.Property(m => m.Id).ValueGeneratedOnAdd();

            // Configura o IdInvestidor para ser index unico
            entity.HasIndex(m => m.IdInvestidor).IsUnique();
        });

        base.OnModelCreating(builder);
    }
}
