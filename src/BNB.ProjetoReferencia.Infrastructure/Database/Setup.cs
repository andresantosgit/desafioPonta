﻿using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BNB.ProjetoReferencia.Infrastructure.Database;

[ExcludeFromCodeCoverage]
public static class Setup
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ClienteContext>(options =>
        {
            options.UseSqlServer(configuration["Sql:ConnectionString"]);
        });

        services.AddDbContext<CarteiraContext>(options =>
        {
            options.UseSqlServer(configuration["Sql:ConnectionString"]);
        });

        //services.AddDbContext<WeatherForecastContext>(options =>
        //{
        //    options.UseSqlite(configuration["Sqlite:ConnectionStringWeatherForecast"]);
        //});

        //services.AddDbContext<ClienteContext>(options =>
        //{
        //    options.UseSqlite(configuration["Sqlite:ConnectionStringCliente"]);
        //});

        //services.AddDbContext<CarteiraContext>(options =>
        //{
        //    options.UseSqlite(configuration["Sqlite:ConnectionStringCarteira"]);
        //});
    }
}

