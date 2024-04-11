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
            entity.HasKey(m => m.IdInvestidor);

            // Configura o IdInvestidor para ser index unico
            entity.HasIndex(m => m.IdInvestidor).IsUnique();

            entity.Property(t => t.IdInvestidor).HasColumnName("id_inv");
            entity.Property(t => t.TipoPessoa).HasColumnName("cod_tip_pes");
            entity.Property(t => t.NomeAcionista).HasColumnName("nom_cli");
            entity.Property(t => t.TipoCustodia).HasColumnName("cod_tip_cst");
            entity.Property(t => t.ValorUnitarioPorAcao).HasColumnName("vr_uni_ac");
            entity.Property(t => t.TotalAcoes).HasColumnName("num_ac");
            entity.Property(t => t.DireitoSubscricao).HasColumnName("num_ac_sub");
            entity.Property(t => t.QuantidadeIntegralizada).HasColumnName("num_int");
            entity.Property(t => t.SaldoIntegralizar).HasColumnName("vr_sld_int");
            entity.Property(t => t.ValorIntegralizar).HasColumnName("vr_int");
        });

        base.OnModelCreating(builder);
    }
}
