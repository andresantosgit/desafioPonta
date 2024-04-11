using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;

/// <summary>
/// Evento de Criação
/// </summary>
/// <param name="Local"></param>
/// <param name="TemperaturaC"></param>
public record CriarWeatherForecastEvent(string Local, int TemperaturaC)
{
    public static implicit operator WeatherForecastEntity(CriarWeatherForecastEvent instance)
        => new()
        {
            TemperaturaC = instance.TemperaturaC,
            Local = instance.Local
        };
}

/// <summary>
/// Evento de atualização de temperatura
/// </summary>
/// <param name="TemperaturaC"></param>
public record AtualizarTemperaturaWeatherForecastEvent(string Local, int TemperaturaC)
{
    public static implicit operator WeatherForecastEntity(AtualizarTemperaturaWeatherForecastEvent instance)
        => new()
        {
            TemperaturaC = instance.TemperaturaC,
            Local = instance.Local
        };
}

/// <summary>
/// Evento de remoção de previsão do tempo
/// </summary>
/// <param name="Local"></param>
public record RemoverWeatherForecastEvent(string Local)
{
    public static implicit operator WeatherForecastEntity(RemoverWeatherForecastEvent instance)
        => new()
        {
            Local = instance.Local
        };
}