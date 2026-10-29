# Multi-stage Dockerfile for OrbitAOS.V6 ASP.NET Core Application
# Optimized for AWS ECS Fargate deployment

# Build stage - Use .NET 6.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

WORKDIR /src

# Copy project file and restore dependencies (layer caching optimization)
COPY OrbitAOS.V6.csproj ./
RUN dotnet restore "OrbitAOS.V6.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "OrbitAOS.V6.csproj" -c Release -o /app/build

# Publish stage
FROM builder AS publish
RUN dotnet publish "OrbitAOS.V6.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - Use .NET 6.0 ASP.NET Core runtime (explicit base image from parameter)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS final

WORKDIR /app

# Create non-root user for security
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app

# Copy published application
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:80 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose application port
EXPOSE 80

# Set entry point
ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
