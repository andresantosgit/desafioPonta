using BNB.ProjetoReferencia.Controllers.v1;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Models;

/// <summary>
/// Modelo de cliente
/// </summary>
public class CobrancaModel
{
    /// <summary>
    /// Construtor padrão
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="entity"></param>
    public CobrancaModel(ControllerBase ctrl, CobrancaEntity entity)
    {
        Calendario = entity.Calendario;
        Revisao = entity.Revisao;
        Loc = entity.Loc;
        Location = entity.Location;
        Status = entity.Status;
        Devedor = entity.Devedor;
        Valor = entity.Valor;
        Chave = entity.Chave;
        TxId = entity.TxId;
        SolicitacaoPagador = entity.SolicitacaoPagador;
        InfoAdicionais = entity.InfoAdicionais;
        PixCopiaECola = entity.PixCopiaECola;

        // Adiciona links HATEOAS ao modelo
        Links["self"] = ctrl.Link<CobrancaController>(
           nameof(CobrancaController.Get), routeValues: new { id = entity.TxId }
        );        
    }

    public Calendario Calendario { get; set; }    
    public int Revisao { get; set; }    
    public Loc Loc { get; set; }    
    public string Location { get; set; }    
    public string Status { get; set; }
    public Devedor Devedor { get; set; }
    public Valor Valor { get; set; }
    public string Chave { get; set; }    
    public string TxId { get; set; }
    public string SolicitacaoPagador { get; set; }    
    public List<InfoAdicional> InfoAdicionais { get; set; }    
    public string PixCopiaECola { get; set; }

/// <summary>
/// Links para ações relacionadas.
/// </summary>
public Dictionary<string, string> Links { get; set; } = new();
}
