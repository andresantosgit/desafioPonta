using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;

public interface IClienteRepository : IBaseRepository<ClienteEntity>
{
    Task<ClienteEntity?> FindByIdInvestidorAsync(string idInvestidor, CancellationToken cancellationToken);
}
