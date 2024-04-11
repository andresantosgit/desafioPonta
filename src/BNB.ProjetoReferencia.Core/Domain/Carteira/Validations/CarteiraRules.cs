using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Validations;

[Service(ServiceLifetime.Scoped, 
    typeof(IRules<CriarCarteiraEvent>),
    typeof(IRules<CancelarCarteiraEvent>),
    typeof(IRules<ExpirarCarteiraEvent>)
    )]
public class CarteiraRules : 
    IRules<CriarCarteiraEvent>,
    IRules<CancelarCarteiraEvent>,
    IRules<ExpirarCarteiraEvent>
{
    private readonly ICarteiraRepository _carteiraRepository;
    private readonly IClienteRepository _clienteRepository;

    public CarteiraRules(ICarteiraRepository carteiraRepository,
                         IClienteRepository clienteRepository)
    {
        _carteiraRepository = carteiraRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<Rules> FactoryAsync(CriarCarteiraEvent @event, CancellationToken cancellationToken)
    {
        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.IdInvestidor, cancellationToken);
        var cliente = await _clienteRepository.FindByIdInvestidorAsync(@event.IdInvestidor, cancellationToken);
        var quantidadeAtual = carteiras.Where(x => x.Status == "Pendente" || x.Status == "Aprovado").Sum(y => y.QuantidadeIntegralizada);

        var rules = Rules.Create()
            .IsTrue("QuantidadeAcoesInvalida", @event.QuantidadeIntegralizada > 0, "Quantidade de ações não pode ser 0.")
            .NotNull("InvestidorNaoEncontrado", cliente, "Investidor não foi encontrado.")
            .IsTrue("QuantidadeAcoesIndisponivel", (quantidadeAtual + @event.QuantidadeIntegralizada) <= cliente?.DireitoSubscricao, $"Você não pode comprar mais ações que o permitido, disponível: {cliente?.DireitoSubscricao - quantidadeAtual}.")
            ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(CancelarCarteiraEvent @event, CancellationToken cancellationToken)
    {
        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.IdInvestidor, cancellationToken);

        var rules = Rules.Create()
            .MinLength("CarteiraSemManifesto", 1, carteiras, "Carteira não possui manifesto.")
            .NotNull("ManifestoNaoEncontrado", carteiras.FirstOrDefault(x => x.Id == @event.Id), "Manifesto não foi encontrado");
            ;

        return rules;
    }

    public async Task<Rules> FactoryAsync(ExpirarCarteiraEvent @event, CancellationToken cancellationToken)
    {
        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.IdInvestidor, cancellationToken);

        var rules = Rules.Create()
            .MinLength("CarteiraSemManifesto", 1, carteiras, "Carteira não possui manifesto.")
            .NotNull("ManifestoNaoEncontrado", carteiras.FirstOrDefault(x => x.Id == @event.Id), "Manifesto não foi encontrado");
        ;

        return rules;
    }
}
