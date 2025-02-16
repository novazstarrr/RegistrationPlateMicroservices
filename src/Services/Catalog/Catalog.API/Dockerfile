#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Services/Catalog/Catalog.API/Catalog.API.csproj", "src/Services/Catalog/Catalog.API/"]
COPY ["src/Services/Catalog/Catalog.Domain/Catalog.Domain.csproj", "src/Services/Catalog/Catalog.Domain/"]
COPY ["src/BuildingBlocks/New folder/WebHost.Customization/WebHost.Customization.csproj", "src/BuildingBlocks/New folder/WebHost.Customization/"]
COPY ["src/BuildingBlocks/EventBus/IntegrationEvents/IntegrationEvents.csproj", "src/BuildingBlocks/EventBus/IntegrationEvents/"]

RUN dotnet restore "src/Services/Catalog/Catalog.API/Catalog.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Catalog/Catalog.API"
RUN dotnet build "Catalog.API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["src/Services/Catalog/Catalog.API/Setup/plates.json", "/app/Setup/"]

RUN mkdir -p /app/Data && \
    chown -R 1000:1000 /app/Data && \
    chmod -R 755 /app/Data

ENTRYPOINT ["dotnet", "Catalog.API.dll"]
