using BNB.ProjetoReferencia.Controllers.v1;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Models;

/// <summary>
/// Modelo de carteira
/// </summary>
public class CarteiraModel
{
    /// <summary>
    /// Construtor padrão
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="entity"></param>
    public CarteiraModel(ControllerBase ctrl, CarteiraEntity entity)
    {
        Id = entity.Id;
        IdInvestidor = entity.IdInvestidor;
        DataCriacao = entity.DataCriacao;
        DataAtualizacao = entity.DataAtualizacao;
        QuantidadeIntegralizada = entity.QuantidadeIntegralizada;
        ValorUnitarioPorAcao = entity.ValorUnitarioPorAcao;
        ValorTotal = entity.ValorTotal;
        Status = entity.Status;

        // Adiciona links HATEOAS ao modelo
        Links["self"] = ctrl.Link<CarteirasController>(
          nameof(CarteirasController.Get), routeValues: new { idInvestidor = entity.IdInvestidor }
        );

        Links["delete"] = ctrl.Link<CarteirasController>(
          nameof(CarteirasController.Delete), routeValues: new { id = entity.Id, idInvestidor = entity.IdInvestidor }
        ); ;


        //Links["patch"] = ctrl.Link<WeatherForecastController>(
        //   nameof(WeatherForecastController.Patch), routeValues: new { }
        //);
        //
        //Links["delete"] = ctrl.Link<WeatherForecastController>(
        //   nameof(WeatherForecastController.Delete), routeValues: new { local = Local }
        //);
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