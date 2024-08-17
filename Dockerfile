FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/Skillitory.Domain/Skillitory.Domain.csproj Skillitory.Domain/
COPY src/Skillitory.Application/Skillitory.Application.csproj Skillitory.Application/
COPY src/Skillitory.Infrastructure/Skillitory.Infrastructure.csproj Skillitory.Infrastructure/
COPY src/Skillitory.Api/Skillitory.Api.csproj Skillitory.Api/
RUN dotnet restore Skillitory.Api/Skillitory.Api.csproj

COPY . .
WORKDIR src/Skillitory.Api
RUN dotnet build Skillitory.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Skillitory.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skillitory.Api.dll"]
