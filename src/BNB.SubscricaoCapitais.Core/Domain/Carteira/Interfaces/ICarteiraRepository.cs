using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;

public interface ICarteiraRepository : IBaseRepository<CarteiraEntity, int>
{
    Task<List<CarteiraEntity>> FindAllAsync(CancellationToken cancellationToken);
    Task<List<CarteiraEntity>> FindAllByIdInvestidorAsync(string idInvestidor, CancellationToken cancellationToken);
    Task<CarteiraEntity?> FindByTxIdAsync(string txId, CancellationToken cancellationToken);
    Task<List<CarteiraEntity>> FindAllByStatusAndDateAsync(string status, CancellationToken cancellationToken);
}
