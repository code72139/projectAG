# Usar la imagen oficial de .NET 9.0 SDK como base para la construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["Project_AG.csproj", "./"]
RUN dotnet restore

# Copiar el resto del código fuente
COPY . .

# Publicar directamente sin build intermedio
RUN dotnet publish "Project_AG.csproj" -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Project_AG.dll"]
