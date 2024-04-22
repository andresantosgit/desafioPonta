using BNB.ProjetoReferencia.Infrastructure.Http.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BNB.ProjetoReferencia.Infrastructure.Https;

public static class Setup
{
    public static void ConfigureHttp(this IServiceCollection services, IConfiguration configuration)
    {
        var httpConfig = configuration
            .GetSection(nameof(HttpConfiguration))
            .Get<List<HttpConfiguration>>();

        foreach (var clientConfig in httpConfig)
        {
            services.AddHttpClient(clientConfig.Name, client =>
            {
                client.BaseAddress = new Uri(clientConfig.BaseAddress);

                if (clientConfig.DefaultHeaders != null)
                {
                    foreach (var header in clientConfig.DefaultHeaders)
                    {
                        if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", header.Value);
                        else
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            });
        }
    }
}
