using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Events;

/// <summary>
/// Evento de criação de carteira
/// </summary>
/// <param name="IdInvestidor"></param>
/// <param name="QuantidadeIntegralizada"></param>
/// <param name="Matricula"></param>
public record CriarCarteiraEvent(string IdInvestidor, int QuantidadeIntegralizada, string Matricula)
{
    public static implicit operator CarteiraEntity(CriarCarteiraEvent instace)
    {
        return new()
        {
            IdInvestidor = instace.IdInvestidor,
            DataCriacao = DateTimeOffset.Now,
            DataAtualizacao = DateTimeOffset.Now,
            QuantidadeIntegralizada = instace.QuantidadeIntegralizada,
            Chave = Guid.NewGuid().ToString(),
            Matricula = instace.Matricula
        };
    }
}

/// <summary>
/// Evento de exclusão de carteira
/// Exclusão logicamente de uma carteira
/// </summary>
/// <param name="Id"></param>
/// <param name="IdInvestidor"></param>
public record CancelarCarteiraEvent(int Id, string IdInvestidor);

/// <summary>
/// Evento de expiração da carteira
/// </summary>
/// <param name="Id"></param>
/// <param name="IdInvestidor"></param>
public record ExpirarCarteiraEvent(int Id, string IdInvestidor);

/// <summary>
/// Evento de expiração da carteira
/// </summary>
/// <param name="Id"></param>
/// <param name="IdInvestidor"></param>
/// <param name="Status"></param>
public record AtualizarCarteiraEvent(int Id, string IdInvestidor, string Status);

/// <summary>
/// Evento de atualiazação de carteira
/// </summary>
public record CallbackEvent(List<PixEntity> listPix);


