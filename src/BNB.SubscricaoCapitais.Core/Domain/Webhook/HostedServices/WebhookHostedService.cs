using BNB.ProjetoReferencia.Core.Common.Extensions;
using BNB.ProjetoReferencia.Core.Domain.Webhook.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BNB.ProjetoReferencia.Core.Domain.Webhook.HostedServices;

public class WebhookHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _urlCallback;
    private Timer _timer = null;

    public WebhookHostedService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _urlCallback = configuration["UrlCallback"];
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
                    var webhookRepository = scope.ServiceProvider.GetRequiredService<IWebhookRepository>();
                    var webhook = await webhookRepository.Update(new Entities.WebhookEntity { WebhookUrl = _urlCallback }, cancellationToken);
                }
            }
            finally
            {
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }
    }
}
