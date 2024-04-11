using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;
using System.ComponentModel.DataAnnotations;

namespace BNB.ProjetoReferencia.Inputs;

/// <summary>
/// Modelo de entrada para criar uma previsão do tempo.
/// </summary>
public record CriarWeatherForecastInput(
    /// <summary>
    /// Local da previsão do tempo.
    /// </summary>
    [Required(ErrorMessage = "O local é obrigatório.")]
    string Local,

    /// <summary>
    /// Temperatura em Celsius.
    /// </summary>
    [Range(-273, ulong.MaxValue, ErrorMessage = "A temperatura deve estar acima de -273 graus Celsius.")]
    int TemperaturaC)
{
    /// <summary>
    /// converte implicitamente um objeto de entrada em um evento de domínio.
    /// </summary>
    /// <param name="instance"></param>
    public static implicit operator CriarWeatherForecastEvent(CriarWeatherForecastInput instance)
        => new(instance.Local, instance.TemperaturaC);
}
