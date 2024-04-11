using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Validations;

[Service(ServiceLifetime.Scoped,
    typeof(IRules<CriarWeatherForecastEvent>),
    typeof(IRules<AtualizarTemperaturaWeatherForecastEvent>),
    typeof(IRules<RemoverWeatherForecastEvent>)
    )]
public class WeatherForecastRules :
    IRules<CriarWeatherForecastEvent>,
    IRules<AtualizarTemperaturaWeatherForecastEvent>,
    IRules<RemoverWeatherForecastEvent>
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public WeatherForecastRules(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    public async Task<Rules> FactoryAsync(CriarWeatherForecastEvent @event, CancellationToken ctx)
    {
        var weather = await _weatherForecastRepository.FindByLocalAsync(@event.Local, ctx);
        var rules = Rules.Create()
            .IsTrue("LocalJaCadastrado", weather == null, "Local já cadastrado.")
            .IsTrue("LocalNaoDefinido", !string.IsNullOrWhiteSpace(@event.Local), "O nome do local precisa ser definido.")
            .IsTrue("TemperaturaInvalida", @event.TemperaturaC >= -273, "A temperatura deve ser maior que o zero absoluto.")
            ;
        return rules;
    }

    public async Task<Rules> FactoryAsync(AtualizarTemperaturaWeatherForecastEvent @event, CancellationToken ctx)
    {
        var weather = await _weatherForecastRepository.FindByLocalAsync(@event.Local, ctx);
        var rules = Rules.Create()
            .IsTrue("LocalNaoCadastrado", weather != null, "Local não está cadastrado.")
            .IsTrue("TemperaturaInvalida", @event.TemperaturaC >= -273, "A temperatura deve ser maior que o zero absoluto.")
            ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(RemoverWeatherForecastEvent @event, CancellationToken ctx)
    {
        var weather = await _weatherForecastRepository.FindByLocalAsync(@event.Local, ctx);
        var rules = Rules.Create()
            .IsTrue("LocalNaoCadastrado", weather != null, "Local não está cadastrado.")
            ;

        return rules;
    }
}
