FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Trading.Api/Trading.Api.csproj", "src/Trading.Api/"]
COPY ["src/Trading.Application/Trading.Application.csproj", "src/Trading.Application/"]
COPY ["src/Trading.Domain/Trading.Domain.csproj", "src/Trading.Domain/"]
COPY ["src/Trading.Infrastructure/Trading.Infrastructure.csproj", "src/Trading.Infrastructure/"]
RUN dotnet restore "src/Trading.Api/Trading.Api.csproj"
COPY . .
WORKDIR "/src/src/Trading.Api"
RUN dotnet build "Trading.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Trading.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trading.Api.dll"]
