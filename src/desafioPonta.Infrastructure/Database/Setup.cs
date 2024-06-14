using desafioPonta.Infrastructure.Database.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace desafioPonta.Infrastructure.Database;

[ExcludeFromCodeCoverage]
public static class Setup
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TarefaContext>(options =>
        {
            options.UseInMemoryDatabase("pontaDatabase"); 
        });

        //services.AddDbContext<TarefaContext>(options =>
        //{            
        //    options.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"]);
        //});

        //services.AddDbContext<UsuarioContext>(options =>
        //{
        //    options.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"]);
        //});

    }
}

