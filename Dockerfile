# Usar la imagen oficial de .NET 9.0 SDK como base para la construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar todo el código fuente primero
COPY . .

# Restaurar, compilar y publicar
RUN dotnet publish "Project_AG.csproj" \
  -c Release \
  -o /app/publish \
  /p:UseAppHost=false


# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Project_AG.dll"]
