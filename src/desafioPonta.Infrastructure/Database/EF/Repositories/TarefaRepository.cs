using desafioPonta.Core.Common.Attributes;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Tarefa.Interfaces;
using desafioPonta.Infrastructure.Database.EF.Context;
using desafioPonta.Infrastructure.Database.EF.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace desafioPonta.Infrastructure.Database.EF.Repositories;

[Service(ServiceLifetime.Scoped, typeof(ITarefaRepository))]
public class TarefaRepository : BaseRepository<TarefaEntity>, ITarefaRepository
{
    public TarefaRepository(TarefaContext tarefaContext) :
        base(tarefaContext)
    {
    }

    public async Task<TarefaEntity?> FindByIdAsync(int id, CancellationToken cancellationToken)
        => await _context.Set
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<TarefaEntity?> FindByTituloNewAsync(string titulo, CancellationToken cancellationToken)
        => await _context.Set
        .FirstOrDefaultAsync(x => x.Titulo == titulo , cancellationToken);
    public async Task<TarefaEntity?> FindByTituloAsync(string titulo, int id, CancellationToken cancellationToken)
        => await _context.Set
        .FirstOrDefaultAsync(x => x.Titulo == titulo && x.Id != id, cancellationToken);
    public async Task<List<TarefaEntity>> FindAllAsync(CancellationToken cancellationToken)
        => await _context.Set
        .ToListAsync(cancellationToken);
    public async Task<List<TarefaEntity>> FindAllByStatusAsync(string status, CancellationToken cancellationToken)
    {
        Enum.TryParse(status, true, out TarefaStatus parsedStatus);
        return await _context.Set
            .Where(x => x.Status == parsedStatus)
            .ToListAsync(cancellationToken);
    }
        
    public async Task<List<TarefaEntity>> FindAllByUsuarioAsync(string usuario, CancellationToken cancellationToken)
        => await _context.Set
        .Where(x => x.Usuario == usuario)
        .ToListAsync(cancellationToken);

    public async Task<List<TarefaEntity>> FindByIntervaloAsync(int intervaloInicial, int intervaloFinal, CancellationToken cancellationToken)
        => await _context.Set
        .Where(t => t.Id >= intervaloInicial && t.Id <= intervaloFinal)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TarefaEntity>> FindAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    => await _context.Set<TarefaEntity>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);    

    public async Task<IEnumerable<TarefaEntity>> FindAllByStatusPagedAsync(string status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        Enum.TryParse(status, true, out TarefaStatus parsedStatus);

        return await _context.Set<TarefaEntity>()
            .Where(t => t.Status == parsedStatus)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
