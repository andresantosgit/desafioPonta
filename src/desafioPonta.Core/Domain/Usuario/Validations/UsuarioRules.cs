using desafioPonta.Core.Common.Attributes;
using desafioPonta.Core.Common.Helper;
using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Events;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace desafioPonta.Core.Domain.Usuario.Validations;

[Service(ServiceLifetime.Scoped,
    typeof(IRules<CriarUsuarioEvent>),
    typeof(IRules<AtualizarSenhaUsuarioEvent>),
    typeof(IRules<ExcluirUsuarioEvent>)
    )]
public class UsuarioRules :
    IRules<CriarUsuarioEvent>,
    IRules<AtualizarSenhaUsuarioEvent>,
    IRules<ExcluirUsuarioEvent>
{    
    private readonly IUsuarioRepository _usuarioRepository;
 
    public UsuarioRules(IUsuarioRepository UsuarioRepository)
    {        
        _usuarioRepository = UsuarioRepository;        
    }
    
    public async Task<Rules> FactoryAsync(CriarUsuarioEvent @event, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);
        var email = await _usuarioRepository.FindByEmailAsync(@event.Email, cancellationToken);

        var rules = Rules.Create()
            .Null("UsuarioJaExiste", usuario, "Usuario já existe.")
            .Null("UsuarioJaExiste", email, "Email já existe.")
            .MaxLength("UsuarioExcedeuCaracteres", 100, @event.Usuario, "Titulo excedeu a quantidade máxima de caracteres")            
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(AtualizarSenhaUsuarioEvent @event, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuario não foi encontrado.")            
        ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(ExcluirUsuarioEvent @event, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.FindAsync(@event.Usuario, cancellationToken);

        var rules = Rules.Create()
            .NotNull("UsuarioNaoEncontrado", usuario, "Usuario não foi encontrado.")
        ;

        return rules;
    }
}
