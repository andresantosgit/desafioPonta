using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Repositories;

[Service(ServiceLifetime.Scoped, typeof(IWeatherForecastRepository))]
public class WeatherForecastRepository : BaseRepository<WeatherForecastEntity, int>, IWeatherForecastRepository
{
    public WeatherForecastRepository(WeatherForecastContext weatherForecastContext) :
        base(weatherForecastContext)
    {
    }

    public async Task<WeatherForecastEntity?> FindByLocalAsync(string local, CancellationToken cancellationToken)
        => await _context.Set.FirstOrDefaultAsync(x => x.Local == local, cancellationToken);
}
