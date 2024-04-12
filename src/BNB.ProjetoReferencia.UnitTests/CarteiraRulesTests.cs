
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Validations;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using Moq;

namespace BNB.ProjetoReferencia.UnitTests;
public class CarteiraRulesTests
{
    private readonly Mock<ICarteiraRepository> _mockCarteiraRepository;
    private readonly Mock<IClienteRepository> _mockClienteRepository;
    private readonly CarteiraRules _rules;

    public CarteiraRulesTests()
    {
        _mockCarteiraRepository = new Mock<ICarteiraRepository>();
        _mockClienteRepository = new Mock<IClienteRepository>();
        _rules = new CarteiraRules(_mockCarteiraRepository.Object, _mockClienteRepository.Object);
    }

    [Fact]
    public async Task FactoryAsync_CriarCarteiraEvent_Sucesso()
    {
        // Arrange
        var cliente = new ClienteEntity()
        {
            IdInvestidor = "014.072.957-72",
            DireitoSubscricao = 100
        };

        var criarEvent = new CriarCarteiraEvent("014.072.957-72", 80);

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CarteiraEntity>());

        _mockClienteRepository.Setup(r => r.FindByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var rules = await _rules.FactoryAsync(criarEvent, CancellationToken.None);

        // Assert
        Assert.False(rules.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_CriarCarteiraEvent_Error_InvestidorNaoEncontrado()
    {
        // Arrange
        var criarEvent = new CriarCarteiraEvent("014.072.957-72", 80);

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CarteiraEntity>());

        _mockClienteRepository.Setup(r => r.FindByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClienteEntity)null);

        // Act
        var rules = await _rules.FactoryAsync(criarEvent, CancellationToken.None);


        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Investidor não foi encontrado.");
    }

    [Fact]
    public async Task FactoryAsync_CriarCarteiraEvent_Error_QuantidadeAcoesIndisponivel()
    {
        // Arrange
        var criarEvent = new CriarCarteiraEvent("014.072.957-72", 80);

        var cliente = new ClienteEntity()
        {
            DireitoSubscricao = 50
        };

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CarteiraEntity>());

        _mockClienteRepository.Setup(r => r.FindByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var rules = await _rules.FactoryAsync(criarEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message.Contains("Você não pode comprar mais ações que o permitido"));
    }

    [Fact]
    public async Task FactoryAsync_CriarCarteiraEvent_Error_QuantidadeAcoesIndisponivelComCarteira()
    {
        // Arrange
        var criarEvent = new CriarCarteiraEvent("014.072.957-72", 11);

        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    IdInvestidor = "014.072.957-72",
                    QuantidadeIntegralizada = 100,
                    Status = "Pendente"
                }
            };

        var cliente = new ClienteEntity()
        {
            DireitoSubscricao = 110
        };

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);

        _mockClienteRepository.Setup(r => r.FindByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var rules = await _rules.FactoryAsync(criarEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message.Contains("Você não pode comprar mais ações que o permitido"));
    }

    [Fact]
    public async Task FactoryAsync_CancelarCarteiraEvent_Sucesso()
    {
        // Arrange
        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    Id = 1,
                    IdInvestidor = "014.072.957-72",
                    QuantidadeIntegralizada = 100
                }
            };

        var excluirEvent = new CancelarCarteiraEvent(1, "014.072.957-72");

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);

        // Act
        var rules = await _rules.FactoryAsync(excluirEvent, CancellationToken.None);

        // Assert
        Assert.False(rules.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_CancelarCarteiraEvent_Erro_CarteiraSemManifesto()
    {
        // Arrange
        var excluirEvent = new CancelarCarteiraEvent(1, "014.072.957-72");

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CarteiraEntity>());

        // Act
        var rules = await _rules.FactoryAsync(excluirEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Carteira não possui manifesto.");
    }

    [Fact]
    public async Task FactoryAsync_CancelarCarteiraEvent_Erro_ManifestoNaoEncontrado()
    {
        // Arrange
        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    Id = 2,
                    IdInvestidor = "014.072.957-72",
                    QuantidadeIntegralizada = 100
                }
            };

        var excluirEvent = new CancelarCarteiraEvent(1, "014.072.957-72");

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);

        // Act
        var rules = await _rules.FactoryAsync(excluirEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Manifesto não foi encontrado");
    }


    [Fact]
    public async Task FactoryAsync_ExpirarCarteiraEvent_Sucesso()
    {
        // Arrange
        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    Id = 1,
                    IdInvestidor = "014.072.957-72",
                    QuantidadeIntegralizada = 100
                }
            };

        var excluirEvent = new CancelarCarteiraEvent(1, "014.072.957-72");

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);

        // Act
        var rules = await _rules.FactoryAsync(excluirEvent, CancellationToken.None);

        // Assert
        Assert.False(rules.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_ExpirarCarteiraEvent_Erro_CarteiraSemManifesto()
    {
        // Arrange
        var excluirEvent = new CancelarCarteiraEvent(1, "014.072.957-72");

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CarteiraEntity>());

        // Act
        var rules = await _rules.FactoryAsync(excluirEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Carteira não possui manifesto.");
    }

    [Fact]
    public async Task FactoryAsync_ExpirarCarteiraEvent_Erro_ManifestoNaoEncontrado()
    {
        // Arrange
        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    Id = 2,
                    IdInvestidor = "014.072.957-72",
                    QuantidadeIntegralizada = 100
                }
            };

        var excluirEvent = new CancelarCarteiraEvent(1, "014.072.957-72");

        _mockCarteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);

        // Act
        var rules = await _rules.FactoryAsync(excluirEvent, CancellationToken.None);

        // Assert
        Assert.True(rules.HasErrors());
        Assert.Contains(rules.Messages, e => e.Message == "Manifesto não foi encontrado");
    }

}