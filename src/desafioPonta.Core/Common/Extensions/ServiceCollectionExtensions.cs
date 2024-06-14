using desafioPonta.Core.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace desafioPonta.Core.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, Assembly assembly)
    {
        var serviceImplementationTypes = assembly.GetTypes().ToArray();
        foreach (var serviceImplementationType in serviceImplementationTypes)
        {
            var attribute = serviceImplementationType.GetCustomAttribute<ServiceAttribute>();
            if (attribute == null) continue;
            var implementationDescriptor = new ServiceDescriptor(serviceImplementationType, serviceImplementationType, attribute.Lifetime);
            services.Add(implementationDescriptor);
            foreach (var serviceType in attribute.ServiceTypes)
            {
                var factoryDescriptor = new ServiceDescriptor(serviceType, i => i.GetRequiredService(serviceImplementationType), attribute.Lifetime);
                services.Add(factoryDescriptor);
            }
        }
    }

    public static void AddCore(this IServiceCollection services)
    {
        services.AddServices(typeof(ServiceCollectionExtensions).Assembly);
    }
}
