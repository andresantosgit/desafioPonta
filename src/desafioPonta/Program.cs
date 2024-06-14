using desafioPonta.Core.Common.Extensions;
using desafioPonta.Filters;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using desafioPonta.Infrastructure.Common.Extensions;
using desafioPonta.Infrastructure.Database;
using desafioPonta.Infrastructure.Database.EF.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do logger Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Adiciona o middleware Serilog para logging de requisi��es HTTP
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Adiciona os servi�os MVC e o suporte a controllers
builder.Services.AddControllers();

// Adiciona o suporte ao endpoint API Explorer
builder.Services.AddEndpointsApiExplorer();

// Configura��o do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "desafioPonta", Version = "v1" });

    // Define o caminho e o nome do arquivo de documenta��o XML.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Adiciona a defini��o de seguran�a JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autentica��o JWT usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Adiciona a exig�ncia de seguran�a JWT Bearer
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// Adiciona o filtro global de exce��es da API
builder.Services.AddMvc(options => options.Filters.Add(typeof(ApiExceptionFilter)));

// Configura��o de autentica��o JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });



// Adiciona a autoriza��o
builder.Services.AddAuthorization();

builder.Services.AddInfrastructure();

// Adiciona os servi�os do Core e da infraestrutura
builder.Services.AddCore();


// Adiciona o HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configura a Database
//builder.Services.ConfigureDatabase(builder.Configuration);

// Configura a Database em mem�ria
builder.Services.AddDbContext<TarefaContext>(options =>
{
    options.UseInMemoryDatabase("pontaDatabase"); // Nome opcional para o banco em mem�ria
});

builder.Services.AddDbContext<UsuarioContext>(options =>
{
    options.UseInMemoryDatabase("pontaDatabase"); // Nome opcional para o banco em mem�ria
});

builder.Services.AddScoped<ICryptoService, CryptoService>();

var app = builder.Build();

// Adicionando o middleware Serilog para logging de requisi��es HTTP
app.UseSerilogRequestLogging();


// Configura��o do pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Logging de solicita��es HTTP com Serilog
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// Middleware de autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

// Define os endpoints da API
app.MapControllers();

// Executa a aplica��o
app.Run();