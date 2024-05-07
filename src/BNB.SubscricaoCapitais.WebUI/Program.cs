using AutoMapper;
using BNB.ProjetoReferencia.WebUI.Filters;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.S095.BNBAuth.Middleware;
using System.ComponentModel.DataAnnotations;
using BNB.ProjetoReferencia.WebUI.Configuration;
using BNB.ProjetoReferencia.Infrastructure.Common.Extensions;
using BNB.ProjetoReferencia.Core.Common.Extensions;
using BNB.ProjetoReferencia.Infrastructure.Https;
using BNB.ProjetoReferencia.Infrastructure.Database;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using BNB.ProjetoReferencia.Infrastructure.Http.MessageHandler;
using BNB.ProjetoReferencia.Infrastructure.Http.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddBNBAuthISKey(options =>
{
    options.KeycloakResourceFile = BNBAuthDefaults.KeycloakResourceFile;
    options.ISKeyUrl = "http://s2iisg01.dreads.bnb:8076/BN.S267.Autorizador";
    options.ISKeySiglaSistema = "493SUBCAP";
});

// Filters
builder.Services.AddScoped<SegurancaAutorizaActionFilter>();
builder.Services.AddScoped<Validate>();

// Configura a Database
builder.Services.ConfigureDatabase(builder.Configuration);

// Configura o Http
builder.Services.ConfigureHttp(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// Configura os serviços da infraestrutura
builder.Services.AddInfrastructure();

builder.Services.AddRouting(options =>
{
    options.AppendTrailingSlash = true;
});

// Adiciona os serviços do Core
builder.Services.AddCore();

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapDefaultControllerRoute();
app.UseRouting();

app.UseAuthorization();

app.UseBNBAuthISKey();
app.UseMvc();

app.Run();
