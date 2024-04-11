using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;
using Microsoft.EntityFrameworkCore;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;

public class CarteiraContext : BaseContext<CarteiraEntity>
{
    public CarteiraContext(DbContextOptions<CarteiraContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CarteiraEntity>(entity =>
        {
            entity.ToTable("sub_carteira");

            // Indica que é a chave primária
            entity.HasKey(m => m.Id);

            // Configura o Id para ser autoincremento
            entity.Property(m => m.Id).ValueGeneratedOnAdd();

            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.IdInvestidor).HasColumnName("id_inv");
            entity.Property(t => t.TxId).HasColumnName("cod_tx");
            entity.Property(t => t.DataCriacao).HasColumnName("dat_cri");
            entity.Property(t => t.DataAtualizacao).HasColumnName("dat_atu");
            entity.Property(t => t.QuantidadeIntegralizada).HasColumnName("num_int");
            entity.Property(t => t.ValorUnitarioPorAcao).HasColumnName("vr_uni_ac");
            entity.Property(t => t.ValorTotal).HasColumnName("vr_int");
            entity.Property(t => t.Status).HasColumnName("cod_sta");
        });

        base.OnModelCreating(builder);
    }
}