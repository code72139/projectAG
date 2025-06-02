# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Configurar NuGet para usar carpetas específicas de Linux
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_HTTP_CACHE_PATH=/root/.local/nuget/v3-cache

# Copiar solo los archivos necesarios para la restauración
COPY ["Project_AG.csproj", "./"]
RUN dotnet restore "./Project_AG.csproj" \
  --runtime linux-x64 \
  --disable-parallel

# Copiar el resto del código fuente
COPY . .

# Publicar la aplicación
RUN dotnet publish Project_AG.csproj --configuration Release --runtime linux-x64 --self-contained false --output ./publish --property:PublishTrimmed=false --property:UseAppHost=false


# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /src/publish .
EXPOSE 8080
EXPOSE 443
ENTRYPOINT ["dotnet", "Project_AG.dll"]
