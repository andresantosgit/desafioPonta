using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Tarefa.Events;
using desafioPonta.Core.Domain.Tarefa.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Validations;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using Moq;
public class TarefaRulesTests
{
    private readonly Mock<ITarefaRepository> _mockTarefaRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly TarefaRules _rules;

    public TarefaRulesTests()
    {
        _mockTarefaRepository = new Mock<ITarefaRepository>();
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _rules = new TarefaRules(_mockTarefaRepository.Object, _mockUsuarioRepository.Object);
    }    

    [Fact]
    public async Task FactoryAsync_CriarTarefaEvent_Valid()
    {
        // Arrange        
        var criarTarefaEvent = new CriarTarefaEvent("Título válido", "Descrição válida", "Usuario");
        var usuario = new UsuarioEntity
        {
            Usuario = "Usuario"
        };

        // Mock do repositório
        _mockUsuarioRepository.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        // Act
        var result = await _rules.FactoryAsync(criarTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_CriarTarefaEvent_TituloExcedeLimite()
    {
        // Arrange        
        var tituloGrande = new string('A', 101);
        var criarTarefaEvent = new CriarTarefaEvent(tituloGrande, "Descrição válida", "Usuario");

        // Act
        var result = await _rules.FactoryAsync(criarTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Titulo excedeu a quantidade máxima de caracteres");
    }

    [Fact]
    public async Task FactoryAsync_CriarTarefaEvent_DescricaoExcedeLimite()
    {
        // Arrange        
        var descricaoGrande = new string('A', 501);
        var criarTarefaEvent = new CriarTarefaEvent("Título válido", descricaoGrande, "Usuario");

        // Act
        var result = await _rules.FactoryAsync(criarTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());        
        Assert.Contains(result.Messages, e => e.Message == "Descrição excedeu a quantidade máxima de caracteres");
    }

    [Fact]
    public async Task FactoryAsync_AtualizarTarefaEvent_Valid()
    {
        // Arrange        
        var atualizarTarefaEvent = new AtualizarTarefaEvent(1, "Novo Título", "Nova Descrição", "Usuario");
        var usuario = new UsuarioEntity
        {
            Usuario = "Usuario"
        };
        var tarefa = new TarefaEntity
        {
            Id = 1,
            Usuario = "Usuario"
        }
        ;


        // Mock do repositório
        _mockUsuarioRepository.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);

        // Act
        var result = await _rules.FactoryAsync(atualizarTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_AtualizarTarefaEvent_TituloExcedeLimite()
    {
        // Arrange        
        var tituloGrande = new string('A', 101);
        var atualizarTarefaEvent = new AtualizarTarefaEvent(1, tituloGrande, "Nova Descrição", "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TarefaEntity());

        // Act
        var result = await _rules.FactoryAsync(atualizarTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Titulo excedeu a quantidade máxima de caracteres");
    }

    [Fact]
    public async Task FactoryAsync_AtualizarTarefaEvent_DescricaoExcedeLimite()
    {
        // Arrange        
        var descricaoGrande = new string('A', 501);
        var atualizarTarefaEvent = new AtualizarTarefaEvent(1, "Novo Título", descricaoGrande, "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TarefaEntity());

        // Act
        var result = await _rules.FactoryAsync(atualizarTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Descrição excedeu a quantidade máxima de caracteres");
    }

    [Fact]
    public async Task FactoryAsync_AtualizarStatusTarefaEvent_Valid()
    {
        // Arrange
        var atualizarStatusTarefaEvent = new AtualizarStatusTarefaEvent(1, "EmAndamento", "Usuario");
        var usuario = new UsuarioEntity
        {
            Usuario = "Usuario"
        }
        ;
        var tarefa = new TarefaEntity
        {
            Id = 1,
            Usuario = "Usuario"
        }
        ;

        // Mock do repositório
        _mockUsuarioRepository.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);

        // Act
        var result = await _rules.FactoryAsync(atualizarStatusTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_AtualizarStatusTarefaEvent_TarefaNaoEncontrada()
    {
        // Arrange        
        var atualizarStatusTarefaEvent = new AtualizarStatusTarefaEvent(1, "Em Andamento", "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TarefaEntity)null);

        // Act
        var result = await _rules.FactoryAsync(atualizarStatusTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Tarefa não foi encontrada.");
    }

    [Fact]
    public async Task FactoryAsync_AtualizarStatusTarefaEvent_StatusInvalido()
    {
        // Arrange
        var atualizarStatusTarefaEvent = new AtualizarStatusTarefaEvent(1, "Status", "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TarefaEntity());

        // Act
        var result = await _rules.FactoryAsync(atualizarStatusTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());        
        Assert.Contains(result.Messages, e => e.Message == "Status informado é inválido.");
    }

    [Fact]
    public async Task FactoryAsync_AndamentoTarefaEvent_Valid()
    {
        // Arrange        
        var andamentoTarefaEvent = new AndamentoTarefaEvent(1, "Usuario");        
        var usuario = new UsuarioEntity
            {
                Usuario = "Usuario"            
            }
        ;
        var tarefa = new TarefaEntity
        {
            Id= 1,
            Usuario = "Usuario"
        }
        ;


        // Mock do repositório
        _mockUsuarioRepository.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);

        // Act
        var result = await _rules.FactoryAsync(andamentoTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_AndamentoTarefaEvent_TarefaNaoEncontrada()
    {
        // Arrange        
        var andamentoTarefaEvent = new AndamentoTarefaEvent(1, "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TarefaEntity)null);

        // Act
        var result = await _rules.FactoryAsync(andamentoTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Tarefa não foi encontrada.");
    }

    [Fact]
    public async Task FactoryAsync_ConclusaoTarefaEvent_Valid()
    {
        // Arrange        
        var conclusaoTarefaEvent = new ConclusaoTarefaEvent(1, "Usuario");
        var usuario = new UsuarioEntity
        {
            Usuario = "Usuario"
        }
        ;
        var tarefa = new TarefaEntity
        {
            Id = 1,
            Usuario = "Usuario"
        }
        ;

        // Mock do repositório
        _mockUsuarioRepository.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);

        // Act
        var result = await _rules.FactoryAsync(conclusaoTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_ConclusaoTarefaEvent_TarefaNaoEncontrada()
    {
        // Arrange        
        var conclusaoTarefaEvent = new ConclusaoTarefaEvent(1, "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TarefaEntity)null);

        // Act
        var result = await _rules.FactoryAsync(conclusaoTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Tarefa não foi encontrada.");
    }

    [Fact]
    public async Task FactoryAsync_ExcluirTarefaEvent_Valid()
    {
        // Arrange        
        var excluirTarefaEvent = new ExcluirTarefaEvent(1, "Usuario");
        var usuario = new UsuarioEntity
        {
            Usuario = "Usuario"
        }
        ;
        var tarefa = new TarefaEntity
        {
            Id = 1,
            Usuario = "Usuario"
        }
        ;

        // Mock do repositório
        _mockUsuarioRepository.Setup(r => r.FindAsync("Usuario", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tarefa);

        // Act
        var result = await _rules.FactoryAsync(excluirTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors());
    }

    [Fact]
    public async Task FactoryAsync_ExcluirTarefaEvent_TarefaNaoEncontrada()
    {
        // Arrange        
        var excluirTarefaEvent = new ExcluirTarefaEvent(1, "Usuario");

        // Mock do repositório
        _mockTarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TarefaEntity)null);

        // Act
        var result = await _rules.FactoryAsync(excluirTarefaEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors());
        Assert.Contains(result.Messages, e => e.Message == "Tarefa não foi encontrada.");
    }
}

