# Use the .NET 7.0 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copiar os arquivos .csproj e restaurar as dependências
COPY ./desafioPonta/*.csproj ./desafioPonta/
COPY ./desafioPonta.Core/*.csproj ./desafioPonta.Core/
COPY ./desafioPonta.Infrastructure/*.csproj ./desafioPonta.Infrastructure/

# Restaurar dependências
RUN dotnet restore ./desafioPonta/desafioPonta.csproj

# Copiar o restante dos arquivos e construir a aplicação
COPY ./desafioPonta/ ./desafioPonta/
COPY ./desafioPonta.Core/ ./desafioPonta.Core/
COPY ./desafioPonta.Infrastructure/ ./desafioPonta.Infrastructure/

WORKDIR /app/desafioPonta
RUN dotnet publish -c Release -o out

# Usar a imagem oficial do ASP.NET runtime para executar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/desafioPonta/out .

ENTRYPOINT ["dotnet", "desafioPonta.dll"]
