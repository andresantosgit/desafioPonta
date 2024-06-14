using desafioPonta.Core.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace desafioPonta.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddServices(typeof(ServiceCollectionExtensions).Assembly);
    }
}

