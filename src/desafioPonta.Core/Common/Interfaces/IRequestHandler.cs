namespace desafioPonta.Core.Common.Interfaces;

public interface IRequestHandler<TIn> where TIn : IDomainEvent
{
    Task Handle(TIn @event, CancellationToken cancellationToken);
}

public interface IRequestHandler<TIn, TOut> where TIn : IDomainEvent
{
    Task<TOut> Handle(TIn @event, CancellationToken cancellationToken);
}
