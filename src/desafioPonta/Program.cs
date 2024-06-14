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

// Configuração do logger Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Adiciona o middleware Serilog para logging de requisições HTTP
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Adiciona os serviços MVC e o suporte a controllers
builder.Services.AddControllers();

// Adiciona o suporte ao endpoint API Explorer
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "desafioPonta", Version = "v1" });

    // Define o caminho e o nome do arquivo de documentação XML.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Adiciona a definição de segurança JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticação JWT usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Adiciona a exigência de segurança JWT Bearer
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

// Adiciona o filtro global de exceções da API
builder.Services.AddMvc(options => options.Filters.Add(typeof(ApiExceptionFilter)));

// Configuração de autenticação JWT
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



// Adiciona a autorização
builder.Services.AddAuthorization();

builder.Services.AddInfrastructure();

// Adiciona os serviços do Core e da infraestrutura
builder.Services.AddCore();


// Adiciona o HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configura a Database
//builder.Services.ConfigureDatabase(builder.Configuration);

// Configura a Database em memória
builder.Services.AddDbContext<TarefaContext>(options =>
{
    options.UseInMemoryDatabase("pontaDatabase"); // Nome opcional para o banco em memória
});

builder.Services.AddDbContext<UsuarioContext>(options =>
{
    options.UseInMemoryDatabase("pontaDatabase"); // Nome opcional para o banco em memória
});

builder.Services.AddScoped<ICryptoService, CryptoService>();

var app = builder.Build();

// Adicionando o middleware Serilog para logging de requisições HTTP
app.UseSerilogRequestLogging();


// Configuração do pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Logging de solicitações HTTP com Serilog
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Define os endpoints da API
app.MapControllers();

// Executa a aplicação
app.Run();