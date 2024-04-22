using BNB.ProjetoReferencia.Core.Common.Helper;

namespace BNB.ProjetoReferencia.Core.Common.Interfaces;

public interface IRules<in TDomaiModel>
{
    Task<Rules> FactoryAsync(TDomaiModel @event, CancellationToken cancellationToken);
}
