using AutoMapper;
using BNB.ProjetoReferencia.WebUI.Filters;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.S095.BNBAuth.Middleware;
using System.ComponentModel.DataAnnotations;
using BNB.ProjetoReferencia.WebUI.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddBNBAuth(options =>
{
    options.ISKeyFilterEnabled = false;
});

//IMapper mapper = AutoMapperConfig.GetMapperConfig().CreateMapper();
//builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IAuthService, AuthService>();

// Filters
builder.Services.AddScoped<SegurancaAutorizaActionFilter>();
//builder.Services.AddScoped<ExceptionActionFilter>();
builder.Services.AddScoped<Validate>();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseBNBAuth();

app.Run();
