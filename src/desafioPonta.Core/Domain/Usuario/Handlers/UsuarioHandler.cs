using desafioPonta.Core.Common.Attributes;
using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Events;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace desafioPonta.Core.Domain.Usuario.Handlers;

[Service(ServiceLifetime.Scoped,
    typeof(IRequestHandler<DomainEvent<CriarUsuarioEvent>, UsuarioEntity>),
    typeof(IRequestHandler<DomainEvent<AtualizarSenhaUsuarioEvent>, UsuarioEntity>),
    typeof(IRequestHandler<DomainEvent<ExcluirUsuarioEvent>, UsuarioEntity>)
    )]
public class UsuarioHandler :
    IRequestHandler<DomainEvent<CriarUsuarioEvent>, UsuarioEntity>,
    IRequestHandler<DomainEvent<AtualizarSenhaUsuarioEvent>, UsuarioEntity>,
    IRequestHandler<DomainEvent<ExcluirUsuarioEvent>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRules<CriarUsuarioEvent> _criarUsuarioEventRules;
    private readonly IRules<AtualizarSenhaUsuarioEvent> _atualizarUsuarioEventRules;
    private readonly IRules<ExcluirUsuarioEvent> _excluirStatusUsuarioEventRules;

    public UsuarioHandler(IUsuarioRepository UsuarioRepository,
                            IRules<CriarUsuarioEvent> criarUsuarioEventRules,
                            IRules<AtualizarSenhaUsuarioEvent> atualizarUsuarioEventRules,
                            IRules<ExcluirUsuarioEvent> excluirStatusUsuarioEventRules)
    {
        _usuarioRepository = UsuarioRepository;
        _criarUsuarioEventRules = criarUsuarioEventRules;       
        _atualizarUsuarioEventRules = atualizarUsuarioEventRules;
        _excluirStatusUsuarioEventRules = excluirStatusUsuarioEventRules;

    }

    public async Task<UsuarioEntity> Handle(DomainEvent<CriarUsuarioEvent> @event, CancellationToken cancellationToken)
    {
        (await _criarUsuarioEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var usuario = (UsuarioEntity)@event.Model;

        var novaUsuario = await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _usuarioRepository.SaveAsync(cancellationToken);

        return novaUsuario;        
    }

    public async Task<UsuarioEntity> Handle(DomainEvent<AtualizarSenhaUsuarioEvent> @event, CancellationToken cancellationToken)
    {
        (await _atualizarUsuarioEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var usuario = await _usuarioRepository.FindAsync(@event.Model.Usuario, cancellationToken);
        
        usuario!.Senha = @event.Model.Senha;        

        var usuarioAtualizado = _usuarioRepository.Update(usuario);
        await _usuarioRepository.SaveAsync(cancellationToken);

        return usuarioAtualizado;
    }
    
    public async Task Handle(DomainEvent<ExcluirUsuarioEvent> @event, CancellationToken cancellationToken)
    {
        (await _excluirStatusUsuarioEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var usuario = await _usuarioRepository.FindAsync(@event.Model.Usuario, cancellationToken);

        _usuarioRepository.Delete(usuario);
        await _usuarioRepository.SaveAsync(cancellationToken);        
    }

}
