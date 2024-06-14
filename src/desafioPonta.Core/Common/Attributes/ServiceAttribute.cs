using Microsoft.Extensions.DependencyInjection;

namespace desafioPonta.Core.Common.Attributes;

public class ServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public Type[] ServiceTypes { get; }

    public ServiceAttribute(ServiceLifetime lifetime, params Type[] serviceTypes)
    {
        Lifetime = lifetime;
        ServiceTypes = serviceTypes;
    }
}

