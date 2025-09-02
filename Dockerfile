# Stage 1: Build - Usa el SDK completo para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los archivos .csproj y el .sln para restaurar las dependencias en una capa separada
COPY ["ADR_T.ProductCatalog.WebApi/ADR_T.ProductCatalog.WebApi.csproj", "ADR_T.ProductCatalog.WebApi/"]
COPY ["ADR_T.ProductCatalog.Application/ADR_T.ProductCatalog.Application.csproj", "ADR_T.ProductCatalog.Application/"]
COPY ["ADR_T.ProductCatalog.Core/ADR_T.ProductCatalog.Core.csproj", "ADR_T.ProductCatalog.Core/"]
COPY ["ADR_T.ProductCatalog.Infrastructure/ADR_T.ProductCatalog.Infrastructure.csproj", "ADR_T.ProductCatalog.Infrastructure/"]
# LÍNEA AÑADIDA: Copiar el proyecto de Tests
COPY ["ADR_T.ProductCatalog.Tests/ADR_T.ProductCatalog.Tests.csproj", "ADR_T.ProductCatalog.Tests/"]
COPY ["ADR_T.ProductCatalog.sln", "."]
RUN dotnet restore "ADR_T.ProductCatalog.sln"

# Copia el resto del código fuente y compila
COPY . .
WORKDIR "/src/ADR_T.ProductCatalog.WebApi"
RUN dotnet build "ADR_T.ProductCatalog.WebApi.csproj" -c Release -o /app/build

# Stage 2: Publish - Publica la aplicación
FROM build AS publish
RUN dotnet publish "ADR_T.ProductCatalog.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final - Usa la imagen de runtime, que es más ligera
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ADR_T.ProductCatalog.WebApi.dll"]