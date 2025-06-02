# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

EXPOSE 80
EXPOSE 5024

COPY ./*.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /src
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "Project_AG.dll"]
