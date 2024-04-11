using BNB.ProjetoReferencia.Core.Common.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;

public class ClienteEntity : Entity<int>
{
    /// <summary>
    /// Tipo de pessoa
    /// pessoa fisica ou juridica
    /// </summary>
    public string TipoPessoa { get; set; } = string.Empty;

    /// <summary>
    /// Id do investidor
    /// CPF ou CNPJ
    /// </summary>
    public string IdInvestidor { get; set; } = string.Empty;

    /// <summary>
    /// Nome do acionista
    /// </summary>
    public string NomeAcionista { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de custodia
    /// Custodia propria
    /// </summary>
    public string TipoCustodia { get; set; } = string.Empty;

    /// <summary>
    /// Valor unitario por ação
    /// </summary>
    public decimal ValorUnitarioPorAcao { get; set; }

    /// <summary>
    /// Total de ações que ele ja possui
    /// </summary>
    public int TotalAcoes { get; set; }

    /// <summary>
    /// Direito de subscrição, quantidade que ele pode comprar
    /// </summary>
    public int DireitoSubscricao { get; set; }

    /// <summary>
    /// Quantidade de ações que ja foram manifestadas a intenção de compra
    /// </summary>
    public int QuantidadeIntegralizada { get; set; }

    /// <summary>
    /// Saldo total de ações que podem manifestar a intenção de compra
    /// </summary>
    public int SaldoIntegralizar { get; set; }

    /// <summary>
    /// Saldo total em dinheiro que pode ser integralizado
    /// </summary>
    public decimal ValorIntegralizar { get; set; }
}
