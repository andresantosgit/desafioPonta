using System.Text.Json;

namespace BNB.ProjetoReferencia.Core.Common.Interfaces;

public record DomainEvent<TModel>(TModel Model) : IDomainEvent
{
    public string Content() => JsonSerializer.Serialize(Model);

    public override string ToString() => typeof(TModel).Name;
}

public interface IDomainEvent
{
    string Content();
}
