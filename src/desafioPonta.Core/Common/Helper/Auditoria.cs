using System.Diagnostics.CodeAnalysis;

namespace desafioPonta.Core.Common.Helper;

/// <summary>
/// Auditoria de produção de eventos
/// </summary>
[ExcludeFromCodeCoverage]
public class Auditoria
{
    /// <summary>
    /// Id do Evento
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Id do Evento correlacionado
    /// </summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>
    /// Autor
    /// </summary>
    public string CriadoPor { get; init; }

    /// <summary>
    /// Data de Criação
    /// </summary>
    public DateTimeOffset CriadoEm { get; init; }

    /// <summary>
    /// Origem do Evento
    /// </summary>
    public string Origem { get; init; }

    /// <summary>
    /// Tipo do Evento
    /// </summary>
    public string TipoEvento { get; init; }

    public Auditoria(Auditoria correlation)
    {
        Id = Guid.NewGuid();
        CorrelationId = correlation.Id;
        CriadoPor = correlation.CriadoPor;
        CriadoEm = correlation.CriadoEm;
        Origem = correlation.Origem;
        TipoEvento = correlation.TipoEvento;
    }

    public Auditoria(Guid evento, Guid? eventoCorrelacionado, string tipoEvento, string criadoPor, DateTimeOffset criadoEm, string origem, string agencia)
    {
        Id = evento;
        CorrelationId = eventoCorrelacionado;
        CriadoPor = criadoPor;
        CriadoEm = criadoEm;
        Origem = origem;
        TipoEvento = tipoEvento;
    }

    public Auditoria()
    {
    }
}
