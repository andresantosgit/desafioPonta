using BNB.ProjetoReferencia.Core.Common.Interfaces;

namespace BNB.ProjetoReferencia.Core.Common.Helper;

public abstract class Entity
{

}

public abstract class Entity<TKey> : Entity
{
    protected Entity(TKey id = default) => Id = id;

    public TKey Id { get; set; }

    internal ICollection<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();
}
