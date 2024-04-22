using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;

public interface ICobrancaRepository
{
    Task<CobrancaEntity> Add(CobrancaEntity entity, CancellationToken ctx);
    Task<bool> AnyById(string id, CancellationToken ctx);
    Task<CobrancaEntity> GetByTxId(string txId, CancellationToken ctx);    
    Task<CobrancaEntity> Update(CobrancaEntity entity, CancellationToken ctx);    

}
