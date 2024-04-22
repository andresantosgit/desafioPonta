using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using System.ComponentModel;
using System.Globalization;

namespace BNB.ProjetoReferencia.Core.Domain.Cliente.Events;

/// <summary>
/// Evento de atualização de cliente
/// </summary>
/// <param name="IdInvestidor"></param>
/// <param name="EnderecoInvestidor"></param>
/// <param name="TelefoneInvestidor"></param>
/// <param name="EmailInvestidor"></param>
/// <param name="Matricula"></param>
public record AtualizarClienteEvent(string IdInvestidor, string EnderecoInvestidor, string TelefoneInvestidor, string EmailInvestidor, string Matricula)
{
    public static implicit operator ClienteEntity(AtualizarClienteEvent instace)
    {
        return new()
        {
            IdInvestidor = instace.IdInvestidor,
            Endereco = instace.EnderecoInvestidor,
            Telefone = instace.TelefoneInvestidor,
            Email = instace.EmailInvestidor,
            DataAtualizacao = DateTimeOffset.Now,            
            Matricula = instace.Matricula
        };
    }
}
