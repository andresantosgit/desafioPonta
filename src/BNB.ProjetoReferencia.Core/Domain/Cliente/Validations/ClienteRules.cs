using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Events;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.Cliente.Validations;

[Service(ServiceLifetime.Scoped,
    typeof(IRules<AtualizarClienteEvent>)
    )]
public class ClienteRules :
    IRules<AtualizarClienteEvent>
{    
    private readonly IClienteRepository _clienteRepository;
 
    public ClienteRules(IClienteRepository clienteRepository)
    {        
        _clienteRepository = clienteRepository;        
    }
    
    public async Task<Rules> FactoryAsync(AtualizarClienteEvent @event, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.FindByIdInvestidorAsync(@event.IdInvestidor, cancellationToken);

        var rules = Rules.Create()
            .NotNull("InvestidorNaoEncontrado", cliente, "Investidor não foi encontrado.")
        ;

        return rules;
    }
}
