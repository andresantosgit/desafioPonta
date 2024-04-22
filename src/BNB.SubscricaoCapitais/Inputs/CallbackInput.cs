using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Models;
using System.ComponentModel.DataAnnotations;

namespace BNB.ProjetoReferencia.Inputs;

/// <summary>
/// Modelo de entrada para receber o callback do Pix.
/// </summary>
public record CallbackInput(List<PixModel> pix)
{
    /// <summary>
    /// converte implicitamente um objeto de entrada em um evento de domínio.
    /// </summary>
    /// <param name="instance"></param>
    public static implicit operator CallbackEvent(CallbackInput instance)
        => new(instance.pix.Select(x=> new PixEntity
        {
            endToEndId = x.endToEndId,
            devolucoes = x.devolucoes != null ? new Devolucoes
            {
                horario = x.devolucoes.horario != null ? new Horario 
                {
                    solicitacao = x.devolucoes.horario.solicitacao
                } : null,
                id = x.devolucoes.id,
                rtrId  = x.devolucoes.rtrId,
                status = x.devolucoes.status,
                valor = x.devolucoes.valor
            } : null,
            horario = x.horario,
            infoPagador = x.infoPagador,    
            valor = x.valor,
            txid = x.txid
        } ).ToList());
}

