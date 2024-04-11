using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Handlers;

public class CarteiraHandler :
    IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity>,
    IRequestHandler<DomainEvent<ExcluirCarteiraEvent>>
{
    private readonly IRules<CriarCarteiraEvent> _criarCarteiraEventRules;
    private readonly IRules<ExcluirCarteiraEvent> _excluirCarteiraEventRules;
    private readonly ICarteiraRepository _carteiraRepository;
    private readonly IClienteRepository _clienteRepository;

    public CarteiraHandler(IRules<CriarCarteiraEvent> criarCarteiraEventRules,
                           IRules<ExcluirCarteiraEvent> excluirCarteiraEventRules,
                           ICarteiraRepository carteiraRepository,
                           IClienteRepository clienteRepository)
    {
        _criarCarteiraEventRules = criarCarteiraEventRules;
        _excluirCarteiraEventRules = excluirCarteiraEventRules;
        _carteiraRepository = carteiraRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<CarteiraEntity> Handle(DomainEvent<CriarCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _criarCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var cliente = await _clienteRepository.FindByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = (CarteiraEntity)@event.Model;

        carteira.ValorUnitarioPorAcao = cliente!.ValorUnitarioPorAcao;
        carteira.ValorTotal = cliente.ValorUnitarioPorAcao * carteira.QuantidadeIntegralizada;
        carteira.Status = "Pendente";

        var novaCarteira = await _carteiraRepository.AddAsync(carteira, cancellationToken);
        await _carteiraRepository.SaveAsync(cancellationToken);

        return novaCarteira;
    }

    public async Task Handle(DomainEvent<ExcluirCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _excluirCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = carteiras.FirstOrDefault(x => x.Id == @event.Model.Id);

        _carteiraRepository.Delete(carteira!);

        await _carteiraRepository.SaveAsync(cancellationToken);
    }
}
