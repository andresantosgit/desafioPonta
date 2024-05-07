using BNB.ProjetoReferencia.Core.Common.Extensions;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.HostedServices;

public class CarteiraHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public CarteiraHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ExecuteAsync(cancellationToken).NoWait();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var carteiraRepository = scope.ServiceProvider.GetRequiredService<ICarteiraRepository>();
                    var cobrancaRepository = scope.ServiceProvider.GetRequiredService<ICobrancaRepository>();
                    var atualizarCarteiraEventHandler = scope.ServiceProvider.GetRequiredService<IRequestHandler<DomainEvent<AtualizarCarteiraEvent>, CarteiraEntity>>();
                    var carteiras = await carteiraRepository.FindAllByStatusAsync("ATIVA", cancellationToken);

                    foreach (var carteira in carteiras)
                    {
                        var retornoCobranca = await cobrancaRepository.GetByTxId(carteira.TxId, cancellationToken);
                        if (retornoCobranca != null && retornoCobranca.TxId != null && retornoCobranca.Status != null)
                        {
                            var evento = new DomainEvent<AtualizarCarteiraEvent>(new(
                                carteira.Id,
                                carteira.IdInvestidor,
                                (retornoCobranca.Status == "ATIVA" && carteira.DataCriacao.Day != DateTime.Now.Day) ? "EXPIRADO" : retornoCobranca.Status));

                            await atualizarCarteiraEventHandler.Handle(evento, cancellationToken);
                        }
                    }
                }
            }
            catch (Exception err)
            {

            }
            finally
            {
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }
    }
}
