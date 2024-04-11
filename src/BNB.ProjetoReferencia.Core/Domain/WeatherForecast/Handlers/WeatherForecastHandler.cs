using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Handlers;

[Service(ServiceLifetime.Scoped,
    typeof(IRequestHandler<DomainEvent<CriarWeatherForecastEvent>, WeatherForecastEntity>),
    typeof(IRequestHandler<DomainEvent<AtualizarTemperaturaWeatherForecastEvent>, WeatherForecastEntity>),
    typeof(IRequestHandler<DomainEvent<RemoverWeatherForecastEvent>>)
    )]
public class WeatherForecastHandler :
    IRequestHandler<DomainEvent<CriarWeatherForecastEvent>, WeatherForecastEntity>,
    IRequestHandler<DomainEvent<AtualizarTemperaturaWeatherForecastEvent>, WeatherForecastEntity>,
    IRequestHandler<DomainEvent<RemoverWeatherForecastEvent>>
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;
    private readonly IRules<CriarWeatherForecastEvent> _criarWeatherForecastEventRules;
    private readonly IRules<AtualizarTemperaturaWeatherForecastEvent> _updateTemperatureWeatherForecastEventRules;
    private readonly IRules<RemoverWeatherForecastEvent> _removerWeatherForecastEventHandler;

    public WeatherForecastHandler(IWeatherForecastRepository weatherForecastRepository,
                                  IRules<CriarWeatherForecastEvent> criarWeatherForecastEventRules,
                                  IRules<AtualizarTemperaturaWeatherForecastEvent> updateTemperatureWeatherForecastEventRules,
                                  IRules<RemoverWeatherForecastEvent> removerWeatherForecastEventHandler)
    {
        _weatherForecastRepository = weatherForecastRepository;
        _criarWeatherForecastEventRules = criarWeatherForecastEventRules;
        _updateTemperatureWeatherForecastEventRules = updateTemperatureWeatherForecastEventRules;
        _removerWeatherForecastEventHandler = removerWeatherForecastEventHandler;
    }

    public async Task<WeatherForecastEntity> Handle(DomainEvent<CriarWeatherForecastEvent> @event, CancellationToken ctx)
    {
        (await _criarWeatherForecastEventRules.FactoryAsync(@event.Model, ctx)).Validate();

        var novaEntidade = await _weatherForecastRepository.AddAsync(@event.Model, ctx);
        await _weatherForecastRepository.SaveAsync(ctx);

        return novaEntidade;
    }

    public async Task<WeatherForecastEntity> Handle(DomainEvent<AtualizarTemperaturaWeatherForecastEvent> @event, CancellationToken ctx)
    {
        (await _updateTemperatureWeatherForecastEventRules.FactoryAsync(@event.Model, ctx)).Validate();

        var weather = await _weatherForecastRepository.FindByLocalAsync(@event.Model.Local, ctx);
        weather!.TemperaturaC = @event.Model.TemperaturaC;

        var entidade = _weatherForecastRepository.Update(weather);
        await _weatherForecastRepository.SaveAsync(ctx);

        return entidade;
    }

    public async Task Handle(DomainEvent<RemoverWeatherForecastEvent> @event, CancellationToken ctx)
    {
        (await _removerWeatherForecastEventHandler.FactoryAsync(@event.Model, ctx)).Validate();

        var weather = await _weatherForecastRepository.FindByLocalAsync(@event.Model.Local, ctx);

        _weatherForecastRepository.Delete(weather!);

        await _weatherForecastRepository.SaveAsync(ctx);
    }
}
