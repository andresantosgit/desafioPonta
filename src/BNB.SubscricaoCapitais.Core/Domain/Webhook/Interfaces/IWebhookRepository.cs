using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Webhook.Entities;

namespace BNB.ProjetoReferencia.Core.Domain.Webhook.Interfaces;

public interface IWebhookRepository 
{
    Task<WebhookEntity> Update(WebhookEntity entity, CancellationToken ctx);    
}
