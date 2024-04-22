using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Handlers;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using Moq;

namespace BNB.ProjetoReferencia.UnitTests;

public class CarteiraHandlerTests
{
    private Mock<ICarteiraRepository> _carteiraRepository;
    private Mock<IClienteRepository> _clienteRepository;
    private Mock<ICobrancaRepository> _cobrancaRepository;
    private Mock<IRules<CriarCarteiraEvent>> _criarCarteiraEventRules;
    private Mock<IRules<CancelarCarteiraEvent>> _cancelarCarteiraEventHandler;
    private Mock<IRules<ExpirarCarteiraEvent>> _expirarCarteiraEventHandler;
    private Mock<IRules<AtualizarCarteiraEvent>> _atualizarCarteiraEventHandler;
    private Mock<Rules> _rules;

    public CarteiraHandler Instance() =>
        new(_carteiraRepository.Object, _clienteRepository.Object, _cobrancaRepository.Object, _criarCarteiraEventRules.Object,  
            _cancelarCarteiraEventHandler.Object, _expirarCarteiraEventHandler.Object, _atualizarCarteiraEventHandler.Object);

    public void ResetMocks()
    {
        _carteiraRepository = new Mock<ICarteiraRepository>();
        _clienteRepository = new Mock<IClienteRepository>();
        _criarCarteiraEventRules = new Mock<IRules<CriarCarteiraEvent>>();
        _cancelarCarteiraEventHandler = new Mock<IRules<CancelarCarteiraEvent>>();
        _expirarCarteiraEventHandler = new Mock<IRules<ExpirarCarteiraEvent>>();
        _atualizarCarteiraEventHandler = new Mock<IRules<AtualizarCarteiraEvent>>();
        _rules = new Mock<Rules>();
    }


    [Fact]
    public async Task Handle_CriarCarteiraEvent()
    {
        // Arrange
        ResetMocks();

        var handler = Instance();
        var domainEvent = new DomainEvent<CriarCarteiraEvent>(new("014.072.957-72", 80, "000000"));

        // Setups
        _criarCarteiraEventRules.Setup(r => r.FactoryAsync(domainEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_rules.Object);
        _clienteRepository.Setup(r => r.FindByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClienteEntity());
        _carteiraRepository.Setup(r => r.AddAsync(It.IsAny<CarteiraEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CarteiraEntity());

        // Act
        var result = await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _carteiraRepository.Verify(r => r.AddAsync(It.IsAny<CarteiraEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _carteiraRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CancelarCarteiraEvent()
    {
        // Arrange
        ResetMocks();

        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    Id = 1,
                    IdInvestidor = "014.072.957-72",
                    Status = "PENDENTE"
                }
            };


        var handler = Instance();
        var domainEvent = new DomainEvent<CancelarCarteiraEvent>(new(1, "014.072.957-72"));

        // Setups
        _cancelarCarteiraEventHandler.Setup(r => r.FactoryAsync(domainEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_rules.Object);
        _carteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);
        _carteiraRepository.Setup(r => r.Update(It.IsAny<CarteiraEntity>()))
            .Returns(carteira.First());
        _carteiraRepository.Setup(r => r.Delete(It.IsAny<CarteiraEntity>()));

        // Act
        var carteiraAtualizada = await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _carteiraRepository.Verify(r => r.Update(It.IsAny<CarteiraEntity>()), Times.Once);
        _carteiraRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(carteiraAtualizada.Status == "CANCELADO");
    }

    [Fact]
    public async Task Handle_ExpirarCarteiraEvent()
    {
        // Arrange
        ResetMocks();

        var carteira = new List<CarteiraEntity>()
            {
                new CarteiraEntity()
                {
                    Id = 1,
                    IdInvestidor = "014.072.957-72"
                }
            };


        var handler = Instance();
        var domainEvent = new DomainEvent<ExpirarCarteiraEvent>(new(1, "014.072.957-72"));

        // Setups
        _expirarCarteiraEventHandler.Setup(r => r.FactoryAsync(domainEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_rules.Object);
        _carteiraRepository.Setup(r => r.FindAllByIdInvestidorAsync("014.072.957-72", It.IsAny<CancellationToken>()))
            .ReturnsAsync(carteira);
        _carteiraRepository.Setup(r => r.Update(It.IsAny<CarteiraEntity>()))
            .Returns(carteira.First());
        _carteiraRepository.Setup(r => r.Delete(It.IsAny<CarteiraEntity>()));

        // Act
        var carteiraAtualizada = await handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _carteiraRepository.Verify(r => r.Update(It.IsAny<CarteiraEntity>()), Times.Once);
        _carteiraRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(carteiraAtualizada.Status == "EXPIRADO");
    }
}