# Dockerfile para SSBJr.TecAgro.Inventario.Server
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/SSBJr.TecAgro.Inventario.Server/SSBJr.TecAgro.Inventario.Server.csproj", "SSBJr.TecAgro.Inventario.Server/"]
COPY ["src/SSBJr.TecAgro.Inventario.Infrastructure/SSBJr.TecAgro.Inventario.Infrastructure.csproj", "SSBJr.TecAgro.Inventario.Infrastructure/"]
COPY ["src/SSBJr.TecAgro.Inventario.Domain/SSBJr.TecAgro.Inventario.Domain.csproj", "SSBJr.TecAgro.Inventario.Domain/"]

# Restore
RUN dotnet restore "SSBJr.TecAgro.Inventario.Server/SSBJr.TecAgro.Inventario.Server.csproj"

# Copiar código fonte
COPY src/ .

# Build
WORKDIR "/src/SSBJr.TecAgro.Inventario.Server"
RUN dotnet build "SSBJr.TecAgro.Inventario.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SSBJr.TecAgro.Inventario.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Criar diretório para storage e logs
RUN mkdir -p /app/storage /app/logs

ENTRYPOINT ["dotnet", "SSBJr.TecAgro.Inventario.Server.dll"]
