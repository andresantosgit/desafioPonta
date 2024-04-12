﻿using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Repositories;

[Service(ServiceLifetime.Scoped, typeof(ICarteiraRepository))]
public class CarteiraRepository : BaseRepository<CarteiraEntity, int>, ICarteiraRepository
{
    public CarteiraRepository(CarteiraContext weatherForecastContext) :
        base(weatherForecastContext)
    {
    }

    public async Task<List<CarteiraEntity>> FindAllByIdInvestidorAsync(string idInvestidor, CancellationToken cancellationToken)
        => await _context.Set.Where(x => x.IdInvestidor == idInvestidor).ToListAsync(cancellationToken);

    public async Task<List<CarteiraEntity>> FindAllAsync(CancellationToken cancellationToken)
        => await _context.Set.ToListAsync(cancellationToken);

    public async Task<List<CarteiraEntity>> FindAllByStatusAndDateAsync(string status, DateTimeOffset minDataCriacao, CancellationToken cancellationToken)
        => await _context.Set.Where(x => x.Status.Equals(status, StringComparison.InvariantCultureIgnoreCase) && x.DataCriacao >= minDataCriacao).ToListAsync(cancellationToken);
}
