using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Handlers;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;
using Moq;

namespace BNB.ProjetoReferencia.UnitTests;

public class WeatherForecastHandlerTests
{
    private Mock<IWeatherForecastRepository> _weatherForecastRepository;
    private Mock<IRules<CriarWeatherForecastEvent>> _criarWeatherForecastEventRules;
    private Mock<IRules<AtualizarTemperaturaWeatherForecastEvent>> _updateTemperatureWeatherForecastEventRules;
    private Mock<IRules<RemoverWeatherForecastEvent>> _removerWeatherForecastEventHandler;
    private Mock<Rules> _rules;

    public WeatherForecastHandler Instance()
    {
        return new WeatherForecastHandler(
            _weatherForecastRepository.Object,
            _criarWeatherForecastEventRules.Object,
            _updateTemperatureWeatherForecastEventRules.Object,
            _removerWeatherForecastEventHandler.Object);
    }

    public void ResetMocks()
    {
        _weatherForecastRepository = new Mock<IWeatherForecastRepository>();
        _criarWeatherForecastEventRules = new Mock<IRules<CriarWeatherForecastEvent>>();
        _updateTemperatureWeatherForecastEventRules = new Mock<IRules<AtualizarTemperaturaWeatherForecastEvent>>();
        _removerWeatherForecastEventHandler = new Mock<IRules<RemoverWeatherForecastEvent>>();
        _rules = new Mock<Rules>();
    }


    [Fact]
    public async Task Handle_CriarWeatherForecastEvent()
    {
        // Arrange
        ResetMocks();


        var handler = Instance();
        var domainEvent = new DomainEvent<CriarWeatherForecastEvent>(new("Fortaleza", 40));

        // Setups
        _criarWeatherForecastEventRules.Setup(r => r.FactoryAsync(domainEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_rules.Object);
        _weatherForecastRepository.Setup(r => r.AddAsync(It.IsAny<WeatherForecastEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherForecastEntity());

        // Act
        var result = await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _weatherForecastRepository.Verify(r => r.AddAsync(It.IsAny<WeatherForecastEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _weatherForecastRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AtualizarTemperaturaWeatherForecastEvent()
    {
        // Arrange
        ResetMocks();

        var handler = Instance();
        var domainEvent = new DomainEvent<AtualizarTemperaturaWeatherForecastEvent>(new("Fortaleza", 40));

        // Setups
        _updateTemperatureWeatherForecastEventRules.Setup(r => r.FactoryAsync(domainEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_rules.Object);
        _weatherForecastRepository.Setup(r => r.Update(It.IsAny<WeatherForecastEntity>()))
            .Returns(new WeatherForecastEntity());
        _weatherForecastRepository.Setup(r => r.FindByLocalAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherForecastEntity());

        // Act
        var result = await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _weatherForecastRepository.Verify(r => r.Update(It.IsAny<WeatherForecastEntity>()), Times.Once);
        _weatherForecastRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RemoverWeatherForecastEvent()
    {
        // Arrange
        ResetMocks();

        var handler = Instance();
        var domainEvent = new DomainEvent<RemoverWeatherForecastEvent>(new("Fortaleza"));

        // Setups
        _removerWeatherForecastEventHandler.Setup(r => r.FactoryAsync(domainEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_rules.Object);
        _weatherForecastRepository.Setup(r => r.Delete(It.IsAny<WeatherForecastEntity>()));

        // Act
        await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _weatherForecastRepository.Verify(r => r.Delete(It.IsAny<WeatherForecastEntity>()), Times.Once);
        _weatherForecastRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}