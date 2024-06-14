using desafioPonta.Core.Common.Helper;

namespace desafioPonta.Core.Domain.Usuario.Entities;

public class UsuarioEntity : Entity
{
    /// <summary>
    /// ID do usuário.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome de usuário.
    /// </summary>
    public string Usuario { get; set; }

    /// <summary>
    /// Endereço de e-mail.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Senha { get; set; }
}
