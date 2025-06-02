# Usar la imagen oficial de .NET 9.0 SDK como base para la construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar todo el código fuente primero
COPY . .

# Restaurar dependencias
RUN dotnet restore "Project_AG.csproj"

# Compilar y publicar
RUN dotnet build "Project_AG.csproj" -c Release -o /app/build -- verbosity maximum
RUN dotnet publish "Project_AG.csproj" -c Release -o /app/publish /p:UseAppHost=false --verbosity maximum


# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Project_AG.dll"]
