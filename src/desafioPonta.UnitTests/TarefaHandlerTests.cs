using Moq;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Tarefa.Events;
using desafioPonta.Core.Domain.Tarefa.Handlers;
using desafioPonta.Core.Common.Helper;
using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Interfaces;
using Microsoft.Extensions.Logging;
using desafioPonta.Core.Domain.Usuario.Entities;

namespace desafioPonta.Tests.Domain.Handlers
{
    public class TarefaHandlerTests
    {
        private Mock<ITarefaRepository> _tarefaRepository;
        private Mock<IRules<RetornarTarefaEvent>> _retornarTarefaEventRules;
        private Mock<IRules<RetornarTarefaUsuarioEvent>> _retornarTarefaUsuarioEventRules;
        private Mock<IRules<CriarTarefaEvent>> _criarTarefaEventRules;
        private Mock<IRules<AtualizarTarefaEvent>> _atualizarTarefaEventRules;
        private Mock<IRules<AtualizarStatusTarefaEvent>> _atualizarStatusTarefaEventRules;
        private Mock<IRules<AndamentoTarefaEvent>> _andamentoStatusTarefaEventRules;
        private Mock<IRules<ConclusaoTarefaEvent>> _conclusaoStatusTarefaEventRules;
        private Mock<IRules<ExcluirTarefaEvent>> _excluirStatusTarefaEventRules;        
        public TarefaHandler Instance() =>
            new (           
                _tarefaRepository.Object,
                _retornarTarefaEventRules.Object,
                _retornarTarefaUsuarioEventRules.Object,
                _criarTarefaEventRules.Object,
                _atualizarTarefaEventRules.Object,
                _atualizarStatusTarefaEventRules.Object,
                _andamentoStatusTarefaEventRules.Object,
                _conclusaoStatusTarefaEventRules.Object,
                _excluirStatusTarefaEventRules.Object
            );

        public void ResetMocks()
        {            
            _tarefaRepository = new Mock<ITarefaRepository>();
            _retornarTarefaEventRules = new Mock<IRules<RetornarTarefaEvent>>();
            _retornarTarefaUsuarioEventRules = new Mock<IRules<RetornarTarefaUsuarioEvent>>();
            _criarTarefaEventRules = new Mock<IRules<CriarTarefaEvent>>();
            _atualizarTarefaEventRules = new Mock<IRules<AtualizarTarefaEvent>>();
            _atualizarStatusTarefaEventRules = new Mock<IRules<AtualizarStatusTarefaEvent>>();
            _andamentoStatusTarefaEventRules = new Mock<IRules<AndamentoTarefaEvent>>();
            _conclusaoStatusTarefaEventRules = new Mock<IRules<ConclusaoTarefaEvent>>();
            _excluirStatusTarefaEventRules = new Mock<IRules<ExcluirTarefaEvent>>();            
        }

        [Fact]
        public async Task Handle_CriarTarefaEvent()
        {
            // Arrange
            ResetMocks();

            var handler = Instance();
            var criarTarefaEvent = new DomainEvent<CriarTarefaEvent>(new CriarTarefaEvent("Titulo", "Descrição", "Usuario"));

            _criarTarefaEventRules.Setup(r => r.FactoryAsync(criarTarefaEvent.Model, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Rules());
            _tarefaRepository.Setup(r => r.AddAsync(It.IsAny<TarefaEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TarefaEntity());

            // Act
            var result = await handler.Handle(criarTarefaEvent, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _tarefaRepository.Verify(r => r.AddAsync(It.IsAny<TarefaEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _tarefaRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AtualizarTarefaEvent()
        {
            // Arrange
            ResetMocks();

            var handler = Instance();
            var tarefaExistente = new TarefaEntity { Id = 1, Titulo = "Tarefa Existente", Descricao = "Descrição", Usuario = "Usuario" };
            var atualizarTarefaEvent = new DomainEvent<AtualizarTarefaEvent>(new AtualizarTarefaEvent(1, "Novo Título", "Nova Descrição", "Novo Usuario"));

            _atualizarTarefaEventRules.Setup(r => r.FactoryAsync(atualizarTarefaEvent.Model, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Rules());
            _tarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaExistente);
            _tarefaRepository.Setup(r => r.Update(It.IsAny<TarefaEntity>()))
                .Returns<TarefaEntity>(t => t);

            // Act
            var result = await handler.Handle(atualizarTarefaEvent, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Novo Título", result.Titulo);
            Assert.Equal("Nova Descrição", result.Descricao);
            Assert.Equal("Novo Usuario", result.Usuario);
            _tarefaRepository.Verify(r => r.Update(It.IsAny<TarefaEntity>()), Times.Once);
            _tarefaRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AtualizarStatusTarefaEvent()
        {
            // Arrange
            ResetMocks();

            var handler = Instance();
            var tarefaExistente = new TarefaEntity { Id = 1, Titulo = "Tarefa Existente", Descricao = "Descrição", Usuario = "Usuario" };
            var atualizarStatusTarefaEvent = new DomainEvent<AtualizarStatusTarefaEvent>(new AtualizarStatusTarefaEvent(1, "Concluida", "Novo Usuario"));

            _atualizarStatusTarefaEventRules.Setup(r => r.FactoryAsync(atualizarStatusTarefaEvent.Model, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Rules());
            _tarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaExistente);
            _tarefaRepository.Setup(r => r.Update(It.IsAny<TarefaEntity>()))
                .Returns<TarefaEntity>(t => t);

            // Act
            var result = await handler.Handle(atualizarStatusTarefaEvent, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TarefaStatus.Concluida, result.Status);
            Assert.Equal("Novo Usuario", result.Usuario);
            _tarefaRepository.Verify(r => r.Update(It.IsAny<TarefaEntity>()), Times.Once);
            _tarefaRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AndamentoTarefaEvent()
        {
            // Arrange
            ResetMocks();

            var handler = Instance();
            var tarefaExistente = new TarefaEntity { Id = 1, Titulo = "Tarefa Existente", Descricao = "Descrição", Usuario = "Usuario" };
            var andamentoTarefaEvent = new DomainEvent<AndamentoTarefaEvent>(new AndamentoTarefaEvent(1, "Novo Usuario"));

            _andamentoStatusTarefaEventRules.Setup(r => r.FactoryAsync(andamentoTarefaEvent.Model, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Rules());
            _tarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaExistente);
            _tarefaRepository.Setup(r => r.Update(It.IsAny<TarefaEntity>()))
                .Returns<TarefaEntity>(t => t);

            // Act
            var result = await handler.Handle(andamentoTarefaEvent, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TarefaStatus.EmAndamento, result.Status);
            Assert.Equal("Novo Usuario", result.Usuario);
            _tarefaRepository.Verify(r => r.Update(It.IsAny<TarefaEntity>()), Times.Once);
            _tarefaRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ConclusaoTarefaEvent()
        {
            // Arrange
            ResetMocks();

            var handler = Instance();
            var tarefaExistente = new TarefaEntity { Id = 1, Titulo = "Tarefa Existente", Descricao = "Descrição", Usuario = "Usuario" };
            var conclusaoTarefaEvent = new DomainEvent<ConclusaoTarefaEvent>(new ConclusaoTarefaEvent(1, "Novo Usuario"));
            
            _conclusaoStatusTarefaEventRules.Setup(r => r.FactoryAsync(conclusaoTarefaEvent.Model, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Rules());
            _tarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaExistente);
            _tarefaRepository.Setup(r => r.Update(It.IsAny<TarefaEntity>()))
                .Returns<TarefaEntity>(t => t);

            // Act
            var result = await handler.Handle(conclusaoTarefaEvent, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TarefaStatus.Concluida, result.Status);
            Assert.Equal("Novo Usuario", result.Usuario);
            _tarefaRepository.Verify(r => r.Update(It.IsAny<TarefaEntity>()), Times.Once);
            _tarefaRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ExcluirTarefaEvent()
        {
            // Arrange
            ResetMocks();

            var handler = Instance();
            var tarefaExistente = new TarefaEntity { Id = 1, Titulo = "Tarefa Existente", Descricao = "Descrição", Usuario = "Usuario" };
            var excluirTarefaEvent = new DomainEvent<ExcluirTarefaEvent>(new ExcluirTarefaEvent(1, "Usuario"));

            _excluirStatusTarefaEventRules.Setup(r => r.FactoryAsync(excluirTarefaEvent.Model, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Rules());
            _tarefaRepository.Setup(r => r.FindByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tarefaExistente);

            // Act
            await handler.Handle(excluirTarefaEvent, CancellationToken.None);

            // Assert
            _tarefaRepository.Verify(r => r.Delete(It.IsAny<TarefaEntity>()), Times.Once);
            _tarefaRepository.Verify(r => r.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
