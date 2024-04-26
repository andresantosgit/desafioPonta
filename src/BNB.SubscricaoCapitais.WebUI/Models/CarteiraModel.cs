using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BNB.SubscricaoCapitais.WebUI.Models;

/// <summary>
/// Modelo de carteira
/// </summary>
public class CarteiraModel
{
    /// <summary>
    /// Construtor padrão
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="carteira"></param>
    /// <param name="cliente"></param>
    public CarteiraModel(ControllerBase ctrl, CarteiraEntity carteira, ClienteEntity? cliente = null)
    {
        Id = carteira.Id;
        IdInvestidor = carteira.IdInvestidor;
        NomeAcionista = cliente?.NomeAcionista ?? string.Empty;
        TipoPessoa = cliente?.TipoPessoa ?? string.Empty;
        TxId = carteira.TxId;
        DataCriacao = carteira.DataCriacao;
        DataAtualizacao = carteira.DataAtualizacao;
        QuantidadeIntegralizada = carteira.QuantidadeIntegralizada;
        ValorUnitarioPorAcao = carteira.ValorUnitarioPorAcao;
        ValorTotal = carteira.ValorTotal;
        Status = carteira.Status;
        PixCopiaECola = carteira.PixCopiaECola;
    }

    /// <summary>
    /// Id da carteira
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id do investidor que efetuou o manifesto
    /// </summary>
    public string IdInvestidor { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    public string NomeAcionista { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Pessoa
    /// </summary>
    public string TipoPessoa { get; set; } = string.Empty;

    /// <summary>
    /// TxId para identificação pagamento PIX
    /// </summary>
    public string TxId { get; set; } = string.Empty;

    /// <summary>
    /// PixCopiaECola para pagamento PIX
    /// </summary>
    public string PixCopiaECola { get; set; } = string.Empty;

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


    /// <summary>
    /// Links para ações relacionadas.
    /// </summary>
    public Dictionary<string, string> Links { get; set; } = new();
}