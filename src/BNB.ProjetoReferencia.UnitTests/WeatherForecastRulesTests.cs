using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Validations;
using Moq;

namespace BNB.ProjetoReferencia.UnitTests;
public class WeatherForecastRulesTests
{
    private readonly Mock<IWeatherForecastRepository> _mockRepository;
    private readonly WeatherForecastRules _rules;

    public WeatherForecastRulesTests()
    {
        _mockRepository = new Mock<IWeatherForecastRepository>();
        _rules = new WeatherForecastRules(_mockRepository.Object);
    }

    [Fact]
    public async Task FactoryAsync_CriarWeatherForecastEvent_Sucesso()
    {
        // Arrange
        var criarEvent = new CriarWeatherForecastEvent("Fortaleza", 40);

        _mockRepository.Setup(r => r.FindByLocalAsync("Fortaleza", It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherForecastEntity)null); // Simula que o local não está cadastrado

        // Act
        var rules = await _rules.FactoryAsync(criarEvent, CancellationToken.None);

        // Assert
        Assert.False(rules.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_CriarWeatherForecastEvent_Error_LocalJaExiste()
    {
        // Arrange
        var criarEvent = new CriarWeatherForecastEvent("Fortaleza", 40);

        _mockRepository.Setup(r => r.FindByLocalAsync("Fortaleza", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherForecastEntity()); // Simula que o local já está cadastrado

        // Act
        var rules = await _rules.FactoryAsync(criarEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Local já cadastrado.");
    }

    [Fact]
    public async Task FactoryAsync_AtualizarTemperaturaWeatherForecastEvent_Sucesso()
    {
        // Arrange
        var atualizarEvent = new AtualizarTemperaturaWeatherForecastEvent("Fortaleza", 40);


        _mockRepository.Setup(r => r.FindByLocalAsync("Fortaleza", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherForecastEntity()); // Simula que o local está cadastrado

        // Act
        var rules = await _rules.FactoryAsync(atualizarEvent, CancellationToken.None);

        // Assert
        Assert.False(rules.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_AtualizarTemperaturaWeatherForecastEvent_Error_LocalNaoExiste()
    {
        // Arrange
        var atualizarEvent = new AtualizarTemperaturaWeatherForecastEvent("Fortaleza", 40);

        _mockRepository.Setup(r => r.FindByLocalAsync("Teste", It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherForecastEntity)null); // Simula que o local não está cadastrado

        // Act
        var rules = await _rules.FactoryAsync(atualizarEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Local não está cadastrado.");
    }

    [Fact]
    public async Task FactoryAsync_RemoverWeatherForecastEvent_Sucesso()
    {
        // Arrange
        var removerEvent = new RemoverWeatherForecastEvent("Fortaleza");

        _mockRepository.Setup(r => r.FindByLocalAsync("Fortaleza", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeatherForecastEntity()); // Simula que o local está cadastrado

        // Act
        var rules = await _rules.FactoryAsync(removerEvent, CancellationToken.None);

        // Assert
        Assert.False(rules.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_RemoverWeatherForecastEvent_Erro_LocalNaoExiste()
    {
        // Arrange
        var removerEvent = new RemoverWeatherForecastEvent("Fortaleza");

        _mockRepository.Setup(r => r.FindByLocalAsync("Teste", It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherForecastEntity)null); // Simula que o local não está cadastrado

        // Act
        var rules = await _rules.FactoryAsync(removerEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Local não está cadastrado.");
    }

}