using desafioPonta.Core.Common.Attributes;
using desafioPonta.Core.Common.Helper;
using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Tarefa.Events;
using desafioPonta.Core.Domain.Tarefa.Interfaces;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace desafioPonta.Core.Domain.Tarefa.Validations;

[Service(ServiceLifetime.Scoped,
    typeof(IRules<RetornarTarefaEvent>),
    typeof(IRules<RetornarTarefaUsuarioEvent>),
    typeof(IRules<CriarTarefaEvent>),
    typeof(IRules<AtualizarTarefaEvent>),
    typeof(IRules<AtualizarStatusTarefaEvent>),
    typeof(IRules<AndamentoTarefaEvent>),
    typeof(IRules<ConclusaoTarefaEvent>),
    typeof(IRules<ExcluirTarefaEvent>)
    )]
public class TarefaRules :
    IRules<RetornarTarefaEvent>,
    IRules<RetornarTarefaUsuarioEvent>,
    IRules<CriarTarefaEvent>,
    IRules<AtualizarTarefaEvent>,
    IRules<AtualizarStatusTarefaEvent>,
    IRules<AndamentoTarefaEvent>,
    IRules<ConclusaoTarefaEvent>,
    IRules<ExcluirTarefaEvent>
{    
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public TarefaRules(ITarefaRepository tarefaRepository, IUsuarioRepository usuarioRepository)
    {        
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Rules> FactoryAsync(RetornarTarefaEvent @event, CancellationToken cancellationToken)
    {
        var rules = Rules.Create();

        if (@event.statusObrigatorio)
        {
            rules
            .NotNull("StatusInvalido", EnumHelper.ValidateStringToEnum<TarefaStatus>(@event.status), "Status informado é inválido.");
        }

        if (!string.IsNullOrEmpty(@event.status))
        {
            rules
            .IsTrue("StatusInvalido", EnumHelper.ValidateStringToEnum<TarefaStatus>(@event.status), "Status informado é inválido.");
        }

        return rules;
    }

    public async Task<Rules> FactoryAsync(RetornarTarefaUsuarioEvent @event, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.FindAsync(@event.usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado");

        return rules;
    }

    public async Task<Rules> FactoryAsync(CriarTarefaEvent @event, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);
        var tarefa = await _tarefaRepository.FindByTituloNewAsync(@event.Titulo, cancellationToken);

        var rules = Rules.Create()
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado")
            .Null("TarefaJaExiste", tarefa, "Tarefa já existe com esse Título.")
            .MaxLength("TituloExcedeuCaracteres", 100, @event.Titulo, "Titulo excedeu a quantidade máxima de caracteres")
            .MaxLength("DescricaoExcedeuCaracteres", 500, @event.Descricao, "Descrição excedeu a quantidade máxima de caracteres")
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(AtualizarTarefaEvent @event, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Id, cancellationToken);
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);
        var tarefaTitulo = await _tarefaRepository.FindByTituloAsync(@event.Titulo, @event.Id, cancellationToken);

        var rules = Rules.Create()
            .NotNull("TarefaNaoEncontrada", tarefa, "Tarefa não foi encontrada.")
            .Null("TarefaJaExiste", tarefaTitulo, "Tarefa já existe com esse Título.")
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado")
            .IsTrue("UsuarioInvalido", usuario != null && tarefa != null && @event.Usuario == tarefa.Usuario, "Usuario informado não pode atualizar essa tarefa.")
            .MaxLength("TituloExcedeuCaracteres", 100, @event.Titulo, "Titulo excedeu a quantidade máxima de caracteres")
            .MaxLength("DescricaoExcedeuCaracteres", 500, @event.Descricao, "Descrição excedeu a quantidade máxima de caracteres")
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(AtualizarStatusTarefaEvent @event, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Id, cancellationToken);
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("TarefaNaoEncontrada", tarefa, "Tarefa não foi encontrada.")
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado")
            .IsTrue("UsuarioInvalido", usuario != null && tarefa != null && @event.Usuario == tarefa.Usuario, "Usuario informado não pode atualizar essa tarefa.")
            .IsTrue("StatusInvalido", EnumHelper.ValidateStringToEnum<TarefaStatus>(@event.Status), "Status informado é inválido.")            
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(AndamentoTarefaEvent @event, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Id, cancellationToken);
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("TarefaNaoEncontrada", tarefa, "Tarefa não foi encontrada.")
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado")
            .IsTrue("UsuarioInvalido", usuario != null && tarefa != null && @event.Usuario == tarefa.Usuario, "Usuario informado não pode atualizar essa tarefa.")
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(ConclusaoTarefaEvent @event, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Id, cancellationToken);
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("TarefaNaoEncontrada", tarefa, "Tarefa não foi encontrada.")
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado")
            .IsTrue("UsuarioInvalido", usuario != null && tarefa != null && @event.Usuario == tarefa.Usuario, "Usuario informado não pode atualizar essa tarefa.")
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(ExcluirTarefaEvent @event, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.FindByIdAsync(@event.Id, cancellationToken);
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("TarefaNaoEncontrada", tarefa, "Tarefa não foi encontrada.")
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuário não foi encontrado")
            .IsTrue("UsuarioInvalido", usuario != null && tarefa != null && @event.Usuario == tarefa.Usuario, "Usuario informado não pode excluir essa tarefa.")
        ;

        return rules;
    }
}
