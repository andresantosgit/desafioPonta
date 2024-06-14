using desafioPonta.Core.Common.Helper;
using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Events;
using desafioPonta.Core.Domain.Usuario.Handlers;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using Moq;

public class UsuarioHandlerTests
{
    private Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private Mock<IRules<CriarUsuarioEvent>> _criarUsuarioEventRulesMock;
    private Mock<IRules<AtualizarSenhaUsuarioEvent>> _atualizarSenhaUsuarioEventRulesMock;
    private Mock<IRules<ExcluirUsuarioEvent>> _excluirUsuarioEventRulesMock;

    public UsuarioHandler Instance() =>
            new (
                _usuarioRepositoryMock.Object,
                _criarUsuarioEventRulesMock.Object,
                _atualizarSenhaUsuarioEventRulesMock.Object,
                _excluirUsuarioEventRulesMock.Object
            );

    public void ResetMocks()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _criarUsuarioEventRulesMock = new Mock<IRules<CriarUsuarioEvent>>();
        _atualizarSenhaUsuarioEventRulesMock = new Mock<IRules<AtualizarSenhaUsuarioEvent>>();
        _excluirUsuarioEventRulesMock = new Mock<IRules<ExcluirUsuarioEvent>>();        
    }

    [Fact]
    public async Task Handle_CriarUsuarioEvent()
    {
        // Arrange
        ResetMocks();
        var handler = Instance();
        var criarUsuarioEvent = new DomainEvent<CriarUsuarioEvent>(new CriarUsuarioEvent("Usuario", "Usuario@ponta.com.br", "123456"));

        // Mock do repositório
        _criarUsuarioEventRulesMock.Setup(r => r.FactoryAsync(criarUsuarioEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Rules());
        _usuarioRepositoryMock.Setup(r => r.AddAsync(It.IsAny<UsuarioEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioEntity());

        // Act        
        var result = await handler.Handle(criarUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _usuarioRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UsuarioEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AtualizarSenhaUsuarioEvent()
    {
        // Arrange
        ResetMocks();
        

        var handler = Instance();
        var usuarioExistente = new UsuarioEntity 
        { 
            Id = 1,
            Usuario = "Usuario",
            Email= "test",
            Senha = "123456"
        };
        var atualizarSenhaUsuarioEvent = new DomainEvent<AtualizarSenhaUsuarioEvent>(new AtualizarSenhaUsuarioEvent("Usuario", "123456@"));        

        // Mock do repositório
        _atualizarSenhaUsuarioEventRulesMock.Setup(r => r.FactoryAsync(atualizarSenhaUsuarioEvent.Model,It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Rules());
        _usuarioRepositoryMock.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioEntity()
            {
                Id = 1,
                Usuario = "Usuario",
                Senha = "123456",
                Email = "Usuario@ponta.com"
            });

        _atualizarSenhaUsuarioEventRulesMock.Setup(r => r.FactoryAsync(atualizarSenhaUsuarioEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Rules());
        _usuarioRepositoryMock.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarioExistente);
        _usuarioRepositoryMock.Setup(r => r.Update(It.IsAny<UsuarioEntity>()))
            .Returns<UsuarioEntity>(t => t);

        // Act        
        var result = await handler.Handle(atualizarSenhaUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);        
        Assert.Equal("Usuario", result.Usuario);
        _usuarioRepositoryMock.Verify(r => r.Update(It.IsAny<UsuarioEntity>()), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExcluirUsuarioEvent()
    {
        // Arrange
        ResetMocks();

        var handler = Instance();
        var excluirUsuarioEvent = new DomainEvent<ExcluirUsuarioEvent>(new ExcluirUsuarioEvent("Usuario"));

        // Mock do repositório
        _excluirUsuarioEventRulesMock.Setup(r => r.FactoryAsync(excluirUsuarioEvent.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Rules());
        _usuarioRepositoryMock.Setup(r => r.FindAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioEntity());

        // Act        
        await handler.Handle(excluirUsuarioEvent, CancellationToken.None);

        // Assert
        _usuarioRepositoryMock.Verify(r => r.Delete(It.IsAny<UsuarioEntity>()), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
