using BNB.ProjetoReferencia.Controllers.v1;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Models;

/// <summary>
/// Modelo de cliente
/// </summary>
public class ClienteModel
{
    /// <summary>
    /// Construtor padrão
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="entity"></param>
    public ClienteModel(ControllerBase ctrl, ClienteEntity entity)
    {
        TipoPessoa = entity.TipoPessoa;
        IdInvestidor = entity.IdInvestidor;
        NomeAcionista = entity.NomeAcionista;
        TipoCustodia = entity.TipoCustodia;
        ValorUnitarioPorAcao = entity.ValorUnitarioPorAcao;
        DireitoSubscricao = entity.DireitoSubscricao;
        QuantidadeIntegralizada = entity.QuantidadeIntegralizada;
        EnderecoInvestidor = entity.EnderecoInvestidor;
        TelefoneInvestidor = entity.TelefoneInvestidor;
        EmailInvestidor = entity.EmailInvestidor;
        Matricula = entity.Matricula;
        DataAtualizacao = entity.DataAtualizacao;

        // Adiciona links HATEOAS ao modelo
        Links["self"] = ctrl.Link<ClientesController>(
           nameof(ClientesController.Get), routeValues: new { id = entity.IdInvestidor }
        );

        //Links["patch"] = ctrl.Link<WeatherForecastController>(
        //   nameof(WeatherForecastController.Patch), routeValues: new { }
        //);
        //
        //Links["delete"] = ctrl.Link<WeatherForecastController>(
        //   nameof(WeatherForecastController.Delete), routeValues: new { local = Local }
        //);
    }

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
    /// Direito de subscrição, quantidade que o acionista pode comprar
    /// </summary>
    public int DireitoSubscricao { get; set; }

    /// <summary>
    /// Quantidade de ações que ja foram manifestadas a intenção de compra
    /// </summary>
    public int QuantidadeIntegralizada { get; set; }

    /// <summary>
    /// Endereco Investidor
    /// </summary>
    public string? EnderecoInvestidor { get; set; } = string.Empty;

    /// <summary>
    /// Telefone Investidor
    /// </summary>
    public string? TelefoneInvestidor { get; set; } = string.Empty;

    /// <summary>
    /// Email Investidor
    /// </summary>
    public string? EmailInvestidor { get; set; } = string.Empty;

    /// <summary>
    /// Matricula de quem realizou à alteração, adicionar campos end, tel e email do investidor
    /// </summary>
    public string? Matricula { get; set; } = string.Empty;

    /// <summary>
    /// Data de atualização do investidor    
    /// </summary>
    public DateTimeOffset? DataAtualizacao { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Links para ações relacionadas.
    /// </summary>
    public Dictionary<string, string> Links { get; set; } = new();
}
