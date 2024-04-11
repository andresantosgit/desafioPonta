using BNB.ProjetoReferencia.Core.Common.Helper;

namespace BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;

public class WeatherForecastEntity : Entity<int>
{
    public int TemperaturaC { get; set; }

    public string Local { get; set; } = string.Empty;
}
