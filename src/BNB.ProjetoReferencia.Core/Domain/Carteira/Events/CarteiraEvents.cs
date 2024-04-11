﻿using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Events;

/// <summary>
/// Evento de criação de carteira
/// </summary>
/// <param name="IdInvestidor"></param>
/// <param name="QuantidadeIntegralizada"></param>
public record CriarCarteiraEvent(string IdInvestidor, int QuantidadeIntegralizada)
{
    public static implicit operator CarteiraEntity(CriarCarteiraEvent instace)
    {
        return new()
        {
            IdInvestidor = instace.IdInvestidor,
            DataCriacao = DateTimeOffset.Now,
            DataAtualizacao = DateTimeOffset.Now,
            QuantidadeIntegralizada = instace.QuantidadeIntegralizada
        };
    }
}

/// <summary>
/// Evento de exclusão de carteira
/// Exclusão logicamente de uma carteira
/// </summary>
/// <param name="Id"></param>
/// <param name="IdInvestidor"></param>
public record ExcluirCarteiraEvent(int Id, string IdInvestidor);
