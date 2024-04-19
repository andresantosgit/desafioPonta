using BNB.ProjetoReferencia.Core.Common.Helper;
using System.Text.Json.Serialization;

namespace BNB.ProjetoReferencia.Core.Domain.Webhook.Entities;


public class WebhookEntity : Entity<int>
{    
    public string WebhookUrl { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Type { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Title { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Status { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Detail { get; set; } = string.Empty;
}
