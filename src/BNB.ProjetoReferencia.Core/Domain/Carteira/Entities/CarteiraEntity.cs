using BNB.ProjetoReferencia.Core.Common.Helper;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;


public class CarteiraEntity : Entity<int>
{
    /// <summary>
    /// Id do investidor que efetuou o manifesto
    /// </summary>
    public string IdInvestidor { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do manifesto
    /// </summary>
    public DateTimeOffset DataCriacao { get; set; }

    /// <summary>
    /// Data de atualização do manifesto
    /// Quando o pagamento sofrer qualquer tipo de alteração essa data é atualizada
    /// </summary>
    public DateTimeOffset DataAtualizacao { get; set; }

    /// <summary>
    /// Quantidade de ações que ele manifestou a intenção de compra
    /// </summary>
    public int QuantidadeIntegralizada { get; set; }

    /// <summary>
    /// Valor unitario por ação
    /// </summary>
    public decimal ValorUnitarioPorAcao { get; set; }

    /// <summary>
    /// Valor total do manifesto
    /// </summary>
    public decimal ValorTotal { get; set; }

    /// <summary>
    /// Status do manifesto
    /// Pendente, Aprovado, Cancelado, Expirado
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
