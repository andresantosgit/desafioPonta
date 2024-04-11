using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;
using Microsoft.EntityFrameworkCore;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;

public class WeatherForecastContext : BaseContext<WeatherForecastEntity>
{
    public WeatherForecastContext(DbContextOptions<WeatherForecastContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<WeatherForecastEntity>(entity =>
        {
            // Indica que é a chave primária
            entity.HasKey(m => m.Id);

            // Configura o Id para ser autoincremento
            entity.Property(m => m.Id).ValueGeneratedOnAdd();
        });

        base.OnModelCreating(builder);
    }
}
