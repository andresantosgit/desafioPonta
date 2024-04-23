using BNB.ProjetoReferencia.Core.Domain.Cliente.Events;
using System.ComponentModel.DataAnnotations;

namespace BNB.ProjetoReferencia.Inputs;

/// <summary>
/// Modelo de entrada para criar uma manifestação de compra de ações.
/// </summary>
public record AtualizarClienteInput(
    /// <summary>
    /// Id do investidor, é usado CPF ou CPNJ
    /// </summary>
    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório.")]
    string IdInvestidor,

    /// <summary>
    /// Endereço do Investidor
    /// </summary>    
    string EnderecoInvestidor,

    /// <summary>
    /// Telefone do Investidor
    /// </summary>    
    string TelefoneInvestidor,

    /// <summary>
    /// Email do Investidor
    /// </summary>    
    string EmailInvestidor,

    /// <summary>
    /// Matricula de quem solciita a atualização
    /// </summary>  
    [Required(ErrorMessage = "Matricula é obrigatório.")]
    string Matricula

    )
{
    /// <summary>
    /// converte implicitamente um objeto de entrada em um evento de domínio.
    /// </summary>
    /// <param name="instance"></param>
    public static implicit operator AtualizarClienteEvent(AtualizarClienteInput instance)
        => new(instance.IdInvestidor, instance.EnderecoInvestidor, instance.TelefoneInvestidor, instance.EmailInvestidor, instance.Matricula);
}
