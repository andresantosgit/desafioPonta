using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;

public interface IWeatherForecastRepository : IBaseRepository<WeatherForecastEntity, int>
{
    Task<WeatherForecastEntity?> FindByLocalAsync(string local, CancellationToken ctx);
}
