using BNB.ProjetoReferencia.Core.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddServices(typeof(ServiceCollectionExtensions).Assembly);
    }
}

