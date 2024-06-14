using desafioPonta.Core.Domain.Usuario.Entities;

namespace desafioPonta.Core.Domain.Usuario.Events;

/// <summary>
/// Evento de criação de um usuário
/// </summary>
/// <param name="Usuario"></param>
/// <param name="Email"></param>
/// <param name="Senha"></param>
public record CriarUsuarioEvent(string Usuario, string Email, string Senha)
{
    public static implicit operator UsuarioEntity(CriarUsuarioEvent instace)
    {
        return new()
        {                       
            Usuario = instace.Usuario,
            Email = instace.Email,
            Senha = instace.Senha
        };
    }
}

/// <summary>
/// Evento de atualização da senha do usuario
/// </summary>
/// <param name="Usuario"></param>
/// <param name="Senha"></param>
public record AtualizarSenhaUsuarioEvent(string Usuario, string Senha);

/// <summary>
/// Evento de exclusão do usuario
/// </summary>
/// <param name="Usuario"></param>
public record ExcluirUsuarioEvent(string Usuario);

