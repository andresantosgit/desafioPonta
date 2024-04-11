using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Repositories;

[Service(ServiceLifetime.Scoped, typeof(IClienteRepository))]
public class ClienteRepository : BaseRepository<ClienteEntity, int>, IClienteRepository
{
    public ClienteRepository(ClienteContext weatherForecastContext) :
        base(weatherForecastContext)
    {
    }

    public async Task<ClienteEntity?> FindByIdInvestidorAsync(string idInvestidor, CancellationToken cancellationToken)
        => await _context.Set.FirstOrDefaultAsync(x => x.IdInvestidor == idInvestidor, cancellationToken);
}
