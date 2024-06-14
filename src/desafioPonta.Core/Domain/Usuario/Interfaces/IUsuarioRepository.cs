using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Tarefa.Entities;
using desafioPonta.Core.Domain.Usuario.Entities;

namespace desafioPonta.Core.Domain.Usuario.Interfaces;

public interface IUsuarioRepository : IBaseRepository<UsuarioEntity>
{
    Task<UsuarioEntity?> FindAsync(string usuario, CancellationToken cancellationToken);
    Task<UsuarioEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ValidateAsync(string usuario, string senha, CancellationToken cancellationToken);
}
