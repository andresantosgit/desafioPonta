using BNB.ProjetoReferencia.Core.Common.Extensions;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
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
                    var expirarCarteiraEventHandler = scope.ServiceProvider.GetRequiredService<IRequestHandler<DomainEvent<ExpirarCarteiraEvent>, CarteiraEntity>>();
                    var carteiras = await carteiraRepository.FindAllByStatusAndDateAsync("PENDENTE", DateTimeOffset.Now.AddHours(-25), cancellationToken);

                    foreach (var carteira in carteiras)
                    {
                        // TODO: Precisamos verificar se a carteira ainda está pendente, chamando a api
                        var evento = new DomainEvent<ExpirarCarteiraEvent>(new (carteira.Id, carteira.IdInvestidor));
                        await expirarCarteiraEventHandler.Handle(evento, cancellationToken);
                    }
                }
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
