namespace desafioPonta.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Modelo representando detalhes de erro para respostas de API.
/// </summary>
[Serializable]
public class ErrorModel
{
    /// <summary>
    /// Título do erro, geralmente uma breve descrição.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; }

    /// <summary>
    /// Código de status HTTP associado ao erro.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; init; }

    /// <summary>
    /// Um URI para um tipo de erro que pode fornecer mais informações.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; }

    /// <summary>
    /// Uma coleção de mensagens de erro. A chave é geralmente o nome do campo que causou o erro.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Messages { get; init; }

    /// <summary>
    /// Rastreamento de pilha do erro. Incluído principalmente para fins de depuração.
    /// </summary>
    /// <remarks>
    /// Em ambientes de produção, considere remover ou ocultar detalhes sensíveis do rastreamento de pilha.
    /// </remarks>
    [JsonPropertyName("stack")]
    public string Stack { get; set; }
}
