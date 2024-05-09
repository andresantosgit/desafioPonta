using BNB.ProjetoReferencia.Core.Common.Extensions;
using BNB.ProjetoReferencia.Core.Domain.Carteira.HostedServices;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Webhook.HostedServices;
using BNB.ProjetoReferencia.Filters;
using BNB.ProjetoReferencia.Infrastructure.Common.Extensions;
using BNB.ProjetoReferencia.Infrastructure.Database;
using BNB.ProjetoReferencia.Infrastructure.Https;
using BNB.S095.BNBAuth.Middleware;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Internal;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Services.AddLogging(configure =>
{
    configure.ClearProviders();
    configure.SetMinimumLevel(LogLevel.Trace);
    configure.AddNLog("NLog.config");
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BNB.Projeto Refer�ncia", Version = "v1" });

    // Define o caminho e o nome do arquivo de documenta��o XML.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Adiciona os coment�rios do arquivo XML � documenta��o do Swagger
    c.IncludeXmlComments(xmlPath);

    // Caso a aplica��o esteja em ambiente de desenvolvimento, adiciona o bot�o de autoriza��o
    //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Description = "Autentica��o JWT usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
    //    Name = "Authorization",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.ApiKey,
    //    Scheme = "Bearer"
    //});

    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        Array.Empty<string>()
    //    }
    //});
});

builder.Services.AddBNBAuthBearer();

// � usada para adicionar um filtro global ao pipeline do MVC em uma aplica��o ASP.NET Core.
// Esse filtro ser� aplicado a todas as a��es de todos os controladores, proporcionando uma
// maneira consistente de tratar certos aspectos das requisi��es, como exce��es, autoriza��o,
// valida��o de modelo, etc., em um �nico lugar.
builder.Services.AddMvc(options => options.Filters.Add(typeof(ApiExceptionFilter)));

// .AddSingleton<ISystemClock, SystemClock>() � usada em aplica��es ASP.NET Core para registrar
// uma implementa��o do ISystemClock no cont�iner de inje��o de depend�ncia como um singleton.
builder.Services.AddSingleton<ISystemClock, SystemClock>();

//  Isso � particularmente �til quando voc� precisa gerar URLs fora dos controladores,
//  como em servi�os ou componentes de middleware.
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
    .AddScoped(x => x.GetRequiredService<IUrlHelperFactory>()
    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

// Configura a Database
builder.Services.ConfigureDatabase(builder.Configuration);

// Configura o Http
builder.Services.ConfigureHttp(builder.Configuration);

builder.Services.AddSingleton<ITokenService, TokenService>();

// Configura os servi�os da infraestrutura
builder.Services.AddInfrastructure();

// Adiciona os servi�os do Core
builder.Services.AddCore();

// Adiciona o hosted service da carteira para valida��o de manifesta��es expiradas
builder.Services.AddHostedService<CarteiraHostedService>();

// Adiciona o hosted service do webhook para cadastro da url
builder.Services.AddHostedService<WebhookHostedService>();

// .AddHttpContextAccessor() � usado em aplica��es ASP.NET Core para adicionar o servi�o
// IHttpContextAccessor ao cont�iner de inje��o de depend�ncia.
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseBNBAuthBearer();

app.UseAuthorization();

app.MapControllers();

app.Run();