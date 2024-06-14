using desafioPonta.Core.Common.Helper;

namespace desafioPonta.Core.Common.Interfaces;

public interface IRules<in TDomaiModel>
{
    Task<Rules> FactoryAsync(TDomaiModel @event, CancellationToken cancellationToken);
}
