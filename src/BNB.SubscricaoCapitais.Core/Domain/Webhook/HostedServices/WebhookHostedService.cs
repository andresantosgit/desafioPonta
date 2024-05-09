using BNB.ProjetoReferencia.Core.Domain.Webhook.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BNB.ProjetoReferencia.Core.Domain.Webhook.HostedServices;

public class WebhookHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _urlCallback;
    private Timer _timer = null;

    public WebhookHostedService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _urlCallback = configuration["UrlCallback"];
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
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
