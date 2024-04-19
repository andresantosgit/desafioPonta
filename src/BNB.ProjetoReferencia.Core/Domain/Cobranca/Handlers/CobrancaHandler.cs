using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.Cobranca.Handlers;

[Service(ServiceLifetime.Scoped,
    typeof(IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity>),
    typeof(IRequestHandler<DomainEvent<CancelarCarteiraEvent>, CarteiraEntity>),
    typeof(IRequestHandler<DomainEvent<ExpirarCarteiraEvent>, CarteiraEntity>)
    )]
public class CobrancaHandler :
    IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity>,
    IRequestHandler<DomainEvent<CancelarCarteiraEvent>, CarteiraEntity>,
    IRequestHandler<DomainEvent<ExpirarCarteiraEvent>, CarteiraEntity>
{
    private readonly ICarteiraRepository _carteiraRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICobrancaRepository _cobrancaRepository;
    private readonly IRules<CriarCarteiraEvent> _criarCarteiraEventRules;
    private readonly IRules<CancelarCarteiraEvent> _excluirCarteiraEventRules;
    private readonly IRules<ExpirarCarteiraEvent> _expirarCarteiraEventRules;

    public CobrancaHandler(ICarteiraRepository carteiraRepository,
                           IClienteRepository clienteRepository,
                           ICobrancaRepository cobrancaRepository, 
                           IRules<CriarCarteiraEvent> criarCarteiraEventRules,
                           IRules<CancelarCarteiraEvent> excluirCarteiraEventRules,
                           IRules<ExpirarCarteiraEvent> expirarCarteiraEventRules)
    {
        _criarCarteiraEventRules = criarCarteiraEventRules;
        _excluirCarteiraEventRules = excluirCarteiraEventRules;
        _carteiraRepository = carteiraRepository;
        _clienteRepository = clienteRepository;
        _expirarCarteiraEventRules = expirarCarteiraEventRules;
        _cobrancaRepository = cobrancaRepository;
    }

    public async Task<CarteiraEntity> Handle(DomainEvent<CriarCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _criarCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var cliente = await _clienteRepository.FindByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = (CarteiraEntity)@event.Model;
        
        carteira.ValorUnitarioPorAcao = cliente!.ValorUnitarioPorAcao;
        carteira.ValorTotal = cliente.ValorUnitarioPorAcao * carteira.QuantidadeIntegralizada;        

        string idInvestidor = String.Join("", System.Text.RegularExpressions.Regex.Split(cliente.IdInvestidor, @"[^\d]"));

        var cobranca = new CobrancaEntity()
        {
            Calendario = new Calendario()
            {
                Expiracao = Convert.ToInt32(Math.Round((new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) - DateTime.Now).TotalSeconds))
            },
            Devedor = new Devedor()
            {                        
                Nome     = cliente.NomeAcionista
            },
            Valor= new Valor()
            {
                Original = carteira.ValorTotal.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ModalidadeAlteracao = 1
            },
            SolicitacaoPagador = "Manifestacao de Compra Acao."            
        };

        if(cliente.IdInvestidor.Length > 15)
        {            
            cobranca.Devedor.Cnpj = idInvestidor;
        }
        else
        {
            cobranca.Devedor.Cpf = idInvestidor;
        }

        var retornoCobranca = await _cobrancaRepository.Add(cobranca, cancellationToken);

        if(retornoCobranca.TxId != null)
        {
            carteira.TxId = retornoCobranca.TxId;
            carteira.Status = retornoCobranca.Status;
            var novaCarteira = await _carteiraRepository.AddAsync(carteira, cancellationToken);
            await _carteiraRepository.SaveAsync(cancellationToken);

            return novaCarteira;
        }
        else
        {
            throw new Exception("Erro ao criar chave PIX.");
        }
        
    }

    public async Task<CarteiraEntity> Handle(DomainEvent<CancelarCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _excluirCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = carteiras.FirstOrDefault(x => x.Id == @event.Model.Id);

        carteira!.Status = "CANCELADO";

        var carteiraAtualizada = _carteiraRepository.Update(carteira);
        await _carteiraRepository.SaveAsync(cancellationToken);

        return carteiraAtualizada;
    }

    public async Task<CarteiraEntity> Handle(DomainEvent<ExpirarCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _expirarCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = carteiras.FirstOrDefault(x => x.Id == @event.Model.Id);

        carteira!.Status = "EXPIRADO";

        var carteiraAtualizada = _carteiraRepository.Update(carteira);
        await _carteiraRepository.SaveAsync(cancellationToken);

        return carteiraAtualizada;

    }

}
