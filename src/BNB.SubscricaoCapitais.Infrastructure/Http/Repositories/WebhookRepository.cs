using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Webhook.Entities;
using BNB.ProjetoReferencia.Core.Domain.Webhook.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BNB.ProjetoReferencia.Infrastructure.Http.Repositories;

[Service(ServiceLifetime.Scoped, typeof(IWebhookRepository))]
public class WebhookRepository
    : IWebhookRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _chave;

    #region Data Records

    internal record WebhookModel(string WebhookUrl, string Type, string Title, string Status, string Detail);    

    #endregion Data Records

    public WebhookRepository(IHttpClientFactory fatory, IConfiguration configuration)
    {
        _httpClient = fatory.CreateClient("cobranca");
        _chave = configuration["CobrancaChave"];        
    }

    public async Task<WebhookEntity> Update(WebhookEntity entity, CancellationToken ctx)
    {        
        var uri = GetUri(_chave);        

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var content = JsonSerializer.Serialize(entity, serializeOptions);        

        using var response = await _httpClient.PutAsync(uri, new StringContent(content), ctx);
        if (!response.IsSuccessStatusCode) return default;

        var model = await response.Content.ReadFromJsonAsync<WebhookModel>(cancellationToken: ctx);
        return Map(model);
    }
       
    internal WebhookEntity Map(WebhookModel model)
    {
        if (model == null) return null;

        return new WebhookEntity
        {
            WebhookUrl = model.WebhookUrl,
            Type = model.Type,
            Title = model.Title, 
            Status = model.Status,
            Detail = model.Detail
        };
    }

    internal WebhookModel Map(WebhookEntity entity)
    {
        if (entity == null) return null;

        return new WebhookModel(
            entity.WebhookUrl,
            entity.Type,
            entity.Title,
            entity.Status,
            entity.Detail
        );
    }

    private Uri GetUri(string chave)
        => new(_httpClient.BaseAddress ?? throw new NotSupportedException(), $"/webhook/{chave}");    

}
