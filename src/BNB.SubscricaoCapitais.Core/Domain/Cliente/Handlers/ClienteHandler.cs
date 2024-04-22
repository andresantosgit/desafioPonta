using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Events;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.Cliente.Handlers;

[Service(ServiceLifetime.Scoped,
    typeof(IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity>)
    )]
public class ClienteHandler :
    IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IRules<AtualizarClienteEvent> _atualizarClienteEventRules;

    public ClienteHandler(IClienteRepository clienteRepository,                                                      
                           IRules<AtualizarClienteEvent> atualizarClienteEventRules)
    {
        _atualizarClienteEventRules = atualizarClienteEventRules;
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteEntity> Handle(DomainEvent<AtualizarClienteEvent> @event, CancellationToken cancellationToken)
    {
        (await _atualizarClienteEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var cliente = await _clienteRepository.FindByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);

        cliente!.Endereco = @event.Model.EnderecoInvestidor;
        cliente!.Telefone = @event.Model.TelefoneInvestidor;
        cliente!.Email = @event.Model.EmailInvestidor;
        cliente!.DataAtualizacao = DateTime.Now;
        cliente!.Matricula = @event.Model.Matricula;

        var clienteAtualizado = _clienteRepository.Update(cliente);
        await _clienteRepository.SaveAsync(cancellationToken);

        return clienteAtualizado;

    }

}
