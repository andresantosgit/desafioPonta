using desafioPonta.Core.Common.Attributes;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using desafioPonta.Infrastructure.Database.EF.Context;
using desafioPonta.Infrastructure.Database.EF.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace desafioPonta.Infrastructure.Database.EF.Repositories;

[Service(ServiceLifetime.Scoped, typeof(IUsuarioRepository))]
public class UsuarioRepository : BaseRepository<UsuarioEntity>, IUsuarioRepository
{
    public UsuarioRepository(UsuarioContext usuarioContext) :
        base(usuarioContext)
    {
    }

    public async Task<UsuarioEntity?> FindAsync(string usuario, CancellationToken cancellationToken)
        => await _context.Set
        .FirstOrDefaultAsync(x => x.Usuario == usuario, cancellationToken);
    public async Task<UsuarioEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        => await _context.Set
        .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<bool> ValidateAsync(string usuario, string senha, CancellationToken cancellationToken)
    {
        var usuarioLogado = await _context.Set<UsuarioEntity>()
            .FirstOrDefaultAsync(x => x.Usuario == usuario && x.Senha == senha, cancellationToken);

        return usuarioLogado != null;
    }
}
