# Stage 1: Base Image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

# Stage 2: Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/Trading.ConsoleLogger/Trading.ConsoleLogger.csproj", "src/Trading.ConsoleLogger/"]
RUN dotnet restore "src/Trading.ConsoleLogger/Trading.ConsoleLogger.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Trading.ConsoleLogger"
RUN dotnet build "Trading.ConsoleLogger.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish Image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Trading.ConsoleLogger.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final Image
FROM base AS final
WORKDIR /app

# Copy published files from the previous stage
COPY --from=publish /app/publish .

# Copy appsettings.json to the container
COPY ["src/Trading.ConsoleLogger/appsettings.json", "."]

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Trading.ConsoleLogger.dll"]