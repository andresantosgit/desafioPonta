using desafioPonta.Core.Domain.Tarefa.Validations;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Events;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using desafioPonta.Core.Domain.Usuario.Validations;
using Moq;

public class UsuarioRulesTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly UsuarioRules _rules;
    public UsuarioRulesTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _rules = new UsuarioRules(_usuarioRepositoryMock.Object);
    }    

    [Fact]
    public async Task FactoryAsync_CriarUsuarioEvent_Valid()
    {
        // Arrange        
        var criarUsuarioEvent = new CriarUsuarioEvent("username", "usuario@ponta.com", "123456");

        // Mock do repositório
        _usuarioRepositoryMock.Setup(r => r.FindAsync(criarUsuarioEvent.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioEntity)null); // Simula que o usuário não existe
        _usuarioRepositoryMock.Setup(r => r.FindByEmailAsync(criarUsuarioEvent.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioEntity)null); // Simula que o email não existe

        // Act
        var result = await _rules.FactoryAsync(criarUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());        
        _usuarioRepositoryMock.Verify(r => r.FindAsync(criarUsuarioEvent.Usuario, It.IsAny<CancellationToken>()), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.FindByEmailAsync(criarUsuarioEvent.Email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FactoryAsync_CriarUsuarioEvent_Invalid()
    {
        // Arrange        
        var criarUsuarioEvent = new CriarUsuarioEvent("username", "usuario@ponta.com", "123456");

        // Mock do repositório
        _usuarioRepositoryMock.Setup(r => r.FindAsync(criarUsuarioEvent.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioEntity()); // Simula que o usuário já existe

        // Act
        var result = await _rules.FactoryAsync(criarUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Single(result.Messages);
        Assert.Equal("UsuarioJaExiste", result.Messages[0].Member);
        _usuarioRepositoryMock.Verify(r => r.FindAsync(criarUsuarioEvent.Usuario, It.IsAny<CancellationToken>()), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.FindByEmailAsync(criarUsuarioEvent.Email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FactoryAsync_AtualizarSenhaUsuarioEvent_Valid()
    {
        // Arrange        
        var atualizarSenhaUsuarioEvent = new AtualizarSenhaUsuarioEvent("Usuario", "Senha");

        // Mock do repositório
        _usuarioRepositoryMock.Setup(r => r.FindAsync(atualizarSenhaUsuarioEvent.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioEntity()); // Simula que o usuário existe

        // Act
        var result = await _rules.FactoryAsync(atualizarSenhaUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
        _usuarioRepositoryMock.Verify(r => r.FindAsync(atualizarSenhaUsuarioEvent.Usuario, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FactoryAsync_AtualizarSenhaUsuarioEvent_Invalid()
    {
        // Arrange        
        var atualizarSenhaUsuarioEvent = new AtualizarSenhaUsuarioEvent("Usuario", "Senha");

        // Mock do repositório
        _usuarioRepositoryMock.Setup(r => r.FindAsync(atualizarSenhaUsuarioEvent.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioEntity)null); // Simula que o usuário não existe

        // Act
        var result = await _rules.FactoryAsync(atualizarSenhaUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Single(result.Messages);
        Assert.Equal("UsuarioNaoEncontrado", result.Messages[0].Member);
        _usuarioRepositoryMock.Verify(r => r.FindAsync(atualizarSenhaUsuarioEvent.Usuario, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FactoryAsync_ExcluirUsuarioEvent_Valid()
    {
        // Arrange        
        var excluirUsuarioEvent = new ExcluirUsuarioEvent("Usuario");

        // Mock do repositório
        _usuarioRepositoryMock.Setup(r => r.FindAsync(excluirUsuarioEvent.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioEntity()); // Simula que o usuário existe

        // Act
        var result = await _rules.FactoryAsync(excluirUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
        _usuarioRepositoryMock.Verify(r => r.FindAsync(excluirUsuarioEvent.Usuario, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FactoryAsync_ExcluirUsuarioEvent_Invalid()
    {
        // Arrange        
        var excluirUsuarioEvent = new ExcluirUsuarioEvent("Usuario");

        // Mock do repositório
        _usuarioRepositoryMock.Setup(r => r.FindAsync(excluirUsuarioEvent.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioEntity)null); // Simula que o usuário não existe

        // Act
        var result = await _rules.FactoryAsync(excluirUsuarioEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Single(result.Messages);
        Assert.Equal("UsuarioNaoEncontrado", result.Messages[0].Member);
        _usuarioRepositoryMock.Verify(r => r.FindAsync(excluirUsuarioEvent.Usuario, It.IsAny<CancellationToken>()), Times.Once);
    }
}
