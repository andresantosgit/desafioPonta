using desafioPonta.Core.Common.Attributes;
using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Tarefa.Events;
using desafioPonta.Core.Domain.Tarefa.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace desafioPonta.Core.Domain.Tarefa.Handlers;

[Service(ServiceLifetime.Scoped,
    typeof(IRequestHandler<DomainEvent<RetornarTarefaEvent>, List<TarefaEntity>>),
    typeof(IRequestHandler<DomainEvent<RetornarTarefaUsuarioEvent>, List<TarefaEntity>>),
    typeof(IRequestHandler<DomainEvent<CriarTarefaEvent>, TarefaEntity>),
    typeof(IRequestHandler<DomainEvent<AtualizarTarefaEvent>, TarefaEntity>),
    typeof(IRequestHandler<DomainEvent<AtualizarStatusTarefaEvent>, TarefaEntity>),
    typeof(IRequestHandler<DomainEvent<AndamentoTarefaEvent>, TarefaEntity>),
    typeof(IRequestHandler<DomainEvent<ConclusaoTarefaEvent>, TarefaEntity>),
    typeof(IRequestHandler<DomainEvent<ExcluirTarefaEvent>>)
    )]
public class TarefaHandler :
    IRequestHandler<DomainEvent<RetornarTarefaEvent>, List<TarefaEntity>>,
    IRequestHandler<DomainEvent<RetornarTarefaUsuarioEvent>, List<TarefaEntity>>,
    IRequestHandler<DomainEvent<CriarTarefaEvent>, TarefaEntity>,
    IRequestHandler<DomainEvent<AtualizarTarefaEvent>, TarefaEntity>,
    IRequestHandler<DomainEvent<AtualizarStatusTarefaEvent>, TarefaEntity>,
    IRequestHandler<DomainEvent<AndamentoTarefaEvent>, TarefaEntity>,
    IRequestHandler<DomainEvent<ConclusaoTarefaEvent>, TarefaEntity>,
    IRequestHandler<DomainEvent<ExcluirTarefaEvent>>
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IRules<RetornarTarefaEvent> _retornarTarefaEventRules;
    private readonly IRules<RetornarTarefaUsuarioEvent> _retornarTarefaUsuarioEventRules;
    private readonly IRules<CriarTarefaEvent> _criarTarefaEventRules;
    private readonly IRules<AtualizarTarefaEvent> _atualizarTarefaEventRules;
    private readonly IRules<AtualizarStatusTarefaEvent> _atualizarStatusTarefaEventRules;
    private readonly IRules<AndamentoTarefaEvent> _andamentoStatusTarefaEventRules;
    private readonly IRules<ConclusaoTarefaEvent> _conclusaoStatusTarefaEventRules;
    private readonly IRules<ExcluirTarefaEvent> _excluirStatusTarefaEventRules;    

    public TarefaHandler(ITarefaRepository tarefaRepository,
                            IRules<RetornarTarefaEvent> retornarTarefaEventRules,
                            IRules<RetornarTarefaUsuarioEvent> retornarTarefaUsuarioEventRules,
                            IRules<CriarTarefaEvent> criarTarefaEventRules,
                            IRules<AtualizarTarefaEvent> atualizarTarefaEventRules,
                            IRules<AtualizarStatusTarefaEvent> atualizarStatusTarefaEventRules,
                            IRules<AndamentoTarefaEvent> andamentoStatusTarefaEventRules,
                            IRules<ConclusaoTarefaEvent> conclusaoStatusTarefaEventRules,
                            IRules<ExcluirTarefaEvent> excluirStatusTarefaEventRules                            
        )
    {        
        _tarefaRepository = tarefaRepository;
        _retornarTarefaEventRules = retornarTarefaEventRules;
        _retornarTarefaUsuarioEventRules = retornarTarefaUsuarioEventRules;
        _criarTarefaEventRules = criarTarefaEventRules;
        _atualizarStatusTarefaEventRules = atualizarStatusTarefaEventRules;
        _atualizarTarefaEventRules = atualizarTarefaEventRules;
        _andamentoStatusTarefaEventRules = andamentoStatusTarefaEventRules;
        _conclusaoStatusTarefaEventRules = conclusaoStatusTarefaEventRules;
        _excluirStatusTarefaEventRules = excluirStatusTarefaEventRules;        

    }

    public async Task<List<TarefaEntity>> Handle(DomainEvent<RetornarTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _retornarTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefas = new List<TarefaEntity>();
        int intervaloInicial = @event.Model.intervaloInicial;
        int intervaloTamanho = @event.Model.intervaloTamanho;
        string status = @event.Model.status;

        // Recupera as tarefas de cada intervalo de chave
        while (true)
        {
            var tarefasIntervalo = await _tarefaRepository.FindByIntervaloAsync(intervaloInicial, intervaloInicial + intervaloTamanho - 1, cancellationToken);
            if (!tarefasIntervalo.Any())
                break;

            tarefas.AddRange(tarefasIntervalo);
            intervaloInicial += intervaloTamanho;
        }
        
        // Aplica os filtros de status, se fornecidos
        if (!string.IsNullOrWhiteSpace(status))
        {
            Enum.TryParse(status, true, out TarefaStatus parsedStatus);
            tarefas = tarefas.Where(x => x.Status == parsedStatus).ToList();
        }

        return tarefas;
    }

    public async Task<List<TarefaEntity>> Handle(DomainEvent<RetornarTarefaUsuarioEvent> @event, CancellationToken cancellationToken)
    {
        (await _retornarTarefaUsuarioEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();        

        var tarefas = await _tarefaRepository.FindAllByUsuarioAsync(@event.Model.usuario, cancellationToken); ;

        return tarefas;
    }

    public async Task<TarefaEntity> Handle(DomainEvent<CriarTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _criarTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefa = (TarefaEntity)@event.Model;

        var novaTarefa = await _tarefaRepository.AddAsync(tarefa, cancellationToken);
        await _tarefaRepository.SaveAsync(cancellationToken);

        return novaTarefa;        
    }

    public async Task<TarefaEntity> Handle(DomainEvent<AtualizarTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _atualizarTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Model.Id, cancellationToken);

        tarefa!.Titulo = @event.Model.Titulo;
        tarefa!.Descricao = @event.Model.Descricao;        
        tarefa!.DataAtualizacao = DateTime.Now;
        tarefa!.Usuario = @event.Model.Usuario;

        var tarefaAtualizado = _tarefaRepository.Update(tarefa);
        await _tarefaRepository.SaveAsync(cancellationToken);

        return tarefaAtualizado;
    }

    public async Task<TarefaEntity> Handle(DomainEvent<AtualizarStatusTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _atualizarStatusTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Model.Id, cancellationToken);

        tarefa!.Status = EnumHelper.ConvertStringToEnum<TarefaStatus>(@event.Model.Status);
        tarefa!.DataAtualizacao = DateTime.Now;
        tarefa!.Usuario = @event.Model.Usuario;

        var tarefaAtualizado = _tarefaRepository.Update(tarefa);
        await _tarefaRepository.SaveAsync(cancellationToken);

        return tarefaAtualizado;
    }

    public async Task<TarefaEntity> Handle(DomainEvent<AndamentoTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _andamentoStatusTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Model.Id, cancellationToken);

        tarefa!.Status = TarefaStatus.EmAndamento;
        tarefa!.DataAtualizacao = DateTime.Now;
        tarefa!.Usuario = @event.Model.Usuario;

        var tarefaAtualizado = _tarefaRepository.Update(tarefa);
        await _tarefaRepository.SaveAsync(cancellationToken);

        return tarefaAtualizado;
    }

    public async Task<TarefaEntity> Handle(DomainEvent<ConclusaoTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _conclusaoStatusTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Model.Id, cancellationToken);

        tarefa!.Status = TarefaStatus.Concluida;
        tarefa!.DataAtualizacao = DateTime.Now;
        tarefa!.Usuario = @event.Model.Usuario;

        var tarefaAtualizado = _tarefaRepository.Update(tarefa);
        await _tarefaRepository.SaveAsync(cancellationToken);

        return tarefaAtualizado;
    }

    public async Task Handle(DomainEvent<ExcluirTarefaEvent> @event, CancellationToken cancellationToken)
    {
        (await _excluirStatusTarefaEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Model.Id, cancellationToken);

        _tarefaRepository.Delete(tarefa);
        await _tarefaRepository.SaveAsync(cancellationToken);        
    }

}
