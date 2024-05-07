using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Exceptions;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Handlers;

[Service(ServiceLifetime.Scoped,
    typeof(IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity>),
    typeof(IRequestHandler<DomainEvent<CancelarCarteiraEvent>, CarteiraEntity>),
    typeof(IRequestHandler<DomainEvent<ExpirarCarteiraEvent>, CarteiraEntity>),
    typeof(IRequestHandler<DomainEvent<CallbackEvent>>),
    typeof(IRequestHandler<DomainEvent<AtualizarCarteiraEvent>, CarteiraEntity>)
    )]
public class CarteiraHandler :
    IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity>,
    IRequestHandler<DomainEvent<CancelarCarteiraEvent>, CarteiraEntity>,
    IRequestHandler<DomainEvent<ExpirarCarteiraEvent>, CarteiraEntity>,
    IRequestHandler<DomainEvent<CallbackEvent>>,
    IRequestHandler<DomainEvent<AtualizarCarteiraEvent>, CarteiraEntity>
{
    private readonly ICarteiraRepository _carteiraRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ICobrancaRepository _cobrancaRepository;
    private readonly IRules<CriarCarteiraEvent> _criarCarteiraEventRules;
    private readonly IRules<CancelarCarteiraEvent> _excluirCarteiraEventRules;
    private readonly IRules<ExpirarCarteiraEvent> _expirarCarteiraEventRules;
    private readonly IRules<AtualizarCarteiraEvent> _atualizarCarteiraEventRules;

    public CarteiraHandler(ICarteiraRepository carteiraRepository,
                           IClienteRepository clienteRepository,
                           ICobrancaRepository cobrancaRepository,
                           IRules<CriarCarteiraEvent> criarCarteiraEventRules,
                           IRules<CancelarCarteiraEvent> excluirCarteiraEventRules,
                           IRules<ExpirarCarteiraEvent> expirarCarteiraEventRules,
                           IRules<AtualizarCarteiraEvent> atualizarCarteiraEventRules)
    {
        _criarCarteiraEventRules = criarCarteiraEventRules;
        _excluirCarteiraEventRules = excluirCarteiraEventRules;
        _carteiraRepository = carteiraRepository;
        _clienteRepository = clienteRepository;
        _expirarCarteiraEventRules = expirarCarteiraEventRules;
        _cobrancaRepository = cobrancaRepository;
        _atualizarCarteiraEventRules = atualizarCarteiraEventRules;
    }

    public async Task<CarteiraEntity> Handle(DomainEvent<CriarCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _criarCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var cliente = await _clienteRepository.FindByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = (CarteiraEntity)@event.Model;
        var datetimeNow = DateTime.Now;

        carteira.ValorUnitarioPorAcao = cliente!.ValorUnitarioPorAcao;
        carteira.ValorTotal = cliente.ValorUnitarioPorAcao * carteira.QuantidadeIntegralizada;

        string idInvestidor = String.Join("", System.Text.RegularExpressions.Regex.Split(cliente.IdInvestidor, @"[^\d]"));

        var cobranca = new CobrancaEntity()
        {
            Calendario = new Calendario()
            {
                Expiracao = Convert.ToInt32(Math.Round((new DateTime(datetimeNow.Year, datetimeNow.Month, datetimeNow.Day, 23, 59, 59) - datetimeNow).TotalSeconds))
            },
            Devedor = new Devedor()
            {
                Nome = cliente.NomeAcionista
            },
            Valor = new Valor()
            {
                Original = carteira.ValorTotal.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ModalidadeAlteracao = 1
            },
            SolicitacaoPagador = "Manifestacao de Compra Acao."
        };

        if (cliente.IdInvestidor.Length > 15)
            cobranca.Devedor.Cnpj = idInvestidor;
        else
            cobranca.Devedor.Cpf = idInvestidor;

        var retornoCobranca = await _cobrancaRepository.Add(cobranca, cancellationToken);

        if (retornoCobranca != null && retornoCobranca.TxId != null && retornoCobranca.Status != null && retornoCobranca.PixCopiaECola != null)
        {
            carteira.TxId = retornoCobranca.TxId;
            carteira.Status = retornoCobranca.Status;
            carteira.PixCopiaECola = retornoCobranca.PixCopiaECola;
            var novaCarteira = await _carteiraRepository.AddAsync(carteira, cancellationToken);
            await _carteiraRepository.SaveAsync(cancellationToken);

            return novaCarteira;
        }

        throw new RulesException("CriacaoPagamentoError", "Erro ao criar chave PIX.");
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

    public async Task Handle(DomainEvent<CallbackEvent> @event, CancellationToken cancellationToken)
    {
        foreach (PixEntity pix in @event.Model.listPix)
        {
            var cobranca = await _cobrancaRepository.GetByTxId(pix.txid, cancellationToken);
            var carteira = await _carteiraRepository.FindByTxIdAsync(pix.txid, cancellationToken);

            if (cobranca != null && carteira != null && cobranca!.Status != null)
            {
                carteira!.Status = cobranca!.Status;
                carteira!.DataAtualizacao = DateTime.Now;
                _carteiraRepository.Update(carteira);
                await _carteiraRepository.SaveAsync(cancellationToken);
            }
        }

    }

    public async Task<CarteiraEntity> Handle(DomainEvent<AtualizarCarteiraEvent> @event, CancellationToken cancellationToken)
    {
        (await _atualizarCarteiraEventRules.FactoryAsync(@event.Model, cancellationToken)).Validate();

        var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(@event.Model.IdInvestidor, cancellationToken);
        var carteira = carteiras.FirstOrDefault(x => x.Id == @event.Model.Id);

        carteira!.Status = @event.Model.Status;
        carteira!.DataAtualizacao = DateTime.Now;

        var carteiraAtualizada = _carteiraRepository.Update(carteira);
        await _carteiraRepository.SaveAsync(cancellationToken);

        return carteiraAtualizada;

    }
}
