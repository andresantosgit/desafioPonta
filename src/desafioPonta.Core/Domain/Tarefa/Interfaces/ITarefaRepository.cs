using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Entities;

namespace desafioPonta.Core.Domain.Tarefa.Interfaces;

public interface ITarefaRepository : IBaseRepository<TarefaEntity>
{
    Task<TarefaEntity?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<TarefaEntity?> FindByTituloNewAsync(string titulo, CancellationToken cancellationToken);
    Task<TarefaEntity?> FindByTituloAsync(string titulo, int id, CancellationToken cancellationToken);
    Task<List<TarefaEntity>> FindAllAsync(CancellationToken cancellationToken);
    Task<List<TarefaEntity>> FindAllByStatusAsync(string status, CancellationToken cancellationToken);
    Task<List<TarefaEntity>> FindAllByUsuarioAsync(string usuario, CancellationToken cancellationToken);
    Task<List<TarefaEntity>> FindByIntervaloAsync(int intervaloInicial, int intervaloFinal, CancellationToken cancellationToken);
    Task<IEnumerable<TarefaEntity>> FindAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<TarefaEntity>> FindAllByStatusPagedAsync(string status, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
