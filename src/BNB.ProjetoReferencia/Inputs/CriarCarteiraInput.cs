using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using System.ComponentModel.DataAnnotations;

namespace BNB.ProjetoReferencia.Inputs;

/// <summary>
/// Modelo de entrada para criar uma manifestação de compra de ações.
/// </summary>
public record CriarCarteiraInput(
    /// <summary>
    /// Id do investidor, é usado CPF ou CPNJ
    /// </summary>
    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório.")]
    string IdInvestidor,

    /// <summary>
    /// Quantidade que o investidor deseja integralizar
    /// </summary>
    [Range(0, ulong.MaxValue, ErrorMessage = "A quantidade deve ser acima de 0.")]
    int QuantidadeIntegralizada)
{
    /// <summary>
    /// converte implicitamente um objeto de entrada em um evento de domínio.
    /// </summary>
    /// <param name="instance"></param>
    public static implicit operator CriarCarteiraEvent(CriarCarteiraInput instance)
        => new(instance.IdInvestidor, instance.QuantidadeIntegralizada);
}
