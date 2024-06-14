using desafioPonta.Core.Domain.Tarefa.Events;
using desafioPonta.Core.Domain.Usuario.Events;
using System.ComponentModel.DataAnnotations;

namespace desafioPonta.Inputs
{
    /// <summary>
    /// Modelo de entrada para criar um usuário.
    /// </summary>
    public record CriarUsuarioInput
    (
        /// <summary>
        /// Nome de usuário
        /// </summary>
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "O nome de usuário deve ter entre 5 e 50 caracteres.")]
        string Usuario,

        /// <summary>
        /// Endereço de e-mail
        /// </summary>
        [Required(ErrorMessage = "O endereço de e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O endereço de e-mail não é válido.")]
        string Email,

        /// <summary>
        /// Senha
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        string Senha
    )
    {
        /// <summary>
        /// Converte implicitamente um objeto de entrada em um modelo de evento de domínio.
        /// </summary>
        /// <param name="instance"></param>
        public static implicit operator CriarUsuarioEvent(CriarUsuarioInput instance)
            => new (instance.Usuario, instance.Email, instance.Senha);
    }

    public record AtualizarUsuarioInput(

        /// <summary>
        /// Nome de usuário
        /// </summary>
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "O nome de usuário deve ter entre 5 e 50 caracteres.")]
        string Usuario,

        /// <summary>
        /// Endereço de e-mail
        /// </summary>
        [Required(ErrorMessage = "O endereço de e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O endereço de e-mail não é válido.")]
        string Email,

        /// <summary>
        /// Senha
        /// </summary>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        string Senha
    )
    {
        /// <summary>
        /// converte implicitamente um objeto de entrada em um evento de domínio.
        /// </summary>
        /// <param name="instance"></param>
        public static implicit operator AtualizarSenhaUsuarioEvent(AtualizarUsuarioInput instance)
            => new(instance.Usuario, instance.Senha);
    }
}
