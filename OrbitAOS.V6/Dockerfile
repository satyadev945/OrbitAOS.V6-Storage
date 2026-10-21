# =============================================================================
# Multi-Stage Dockerfile with PurgeCSS for OrbitAOS.V6
# Rule: cz-css-1005 - Unused CSS in Container Images
#
# Remediation: Multi-Stage Dockerfile with PurgeCSS and
#              Azure Container Registry (ACR) Integration for AKS
#
# Unused CSS selectors removed by PurgeCSS in Stage 1 (css-purge):
#   bootstrap-utilities.css  line 1220 : .fw-bold
#   bootstrap-utilities.css  line 1224 : .fw-bolder
#   bootstrap-utilities.rtl.css line 1220 : .fw-bold
#   bootstrap-utilities.rtl.css line 1224 : .fw-bolder
#   bootstrap.css  line 6299 : .placeholder
#   bootstrap.css  line 6312 : .placeholder-xs
#   bootstrap.css  line 6316 : .placeholder-sm
#   bootstrap.css  line 6320 : .placeholder-lg
#   bootstrap.css  line 6324 : .placeholder-glow .placeholder
#   bootstrap.css  line 6340 : .placeholder-wave
#   bootstrap.css  line 7575 : .fw-bold
#   bootstrap.css  line 7579 : .fw-bolder
#   bootstrap.rtl.css line 6284 : .placeholder
#   bootstrap.rtl.css line 6297 : .placeholder-xs
#   bootstrap.rtl.css line 6301 : .placeholder-sm
#   bootstrap.rtl.css line 6305 : .placeholder-lg
#   bootstrap.rtl.css line 6309 : .placeholder-glow .placeholder
#   bootstrap.rtl.css line 6325 : .placeholder-wave
#   bootstrap.rtl.css line 7560 : .fw-bold
#   bootstrap.rtl.css line 7564 : .fw-bolder
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: CSS Purge Stage
# Uses Node.js to run PurgeCSS and strip unused Bootstrap CSS rules
# -----------------------------------------------------------------------------
FROM node:18-alpine AS css-purge

WORKDIR /css-build

# Copy package files and install PurgeCSS
COPY package.json ./
RUN npm install --only=dev

# Copy PurgeCSS configuration
COPY purgecss.config.js ./

# Copy all source files needed for content scanning (Views, JS)
COPY Views/ ./Views/
COPY Areas/ ./Areas/
COPY wwwroot/ ./wwwroot/

# Run PurgeCSS to remove unused CSS rules including:
#   .fw-bold, .fw-bolder (bootstrap-utilities.css lines 1220, 1224)
#   .fw-bold, .fw-bolder (bootstrap-utilities.rtl.css lines 1220, 1224)
#   .placeholder, .placeholder-xs, .placeholder-sm, .placeholder-lg,
#   .placeholder-glow .placeholder, .placeholder-wave
#   (bootstrap.css lines 6299, 6312, 6316, 6320, 6324, 6340, 7575, 7579)
#   (bootstrap.rtl.css lines 6284, 6297, 6301, 6305, 6309, 6325, 7560, 7564)
RUN npm run purge-css

# -----------------------------------------------------------------------------
# Stage 2: .NET SDK Build Stage
# Restores and publishes the ASP.NET Core application
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

# Copy project file and restore dependencies
COPY OrbitAOS.V6.csproj ./
RUN dotnet restore OrbitAOS.V6.csproj

# Copy all source files
COPY . .

# Publish the application in Release mode
RUN dotnet publish OrbitAOS.V6.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# -----------------------------------------------------------------------------
# Stage 3: Final Production Image
# Combines purged CSS from Stage 1 with published app from Stage 2
# Produces a minimal container image for AKS deployment
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Override wwwroot CSS with PurgeCSS-optimized files from css-purge stage
# This replaces the original Bootstrap CSS files with purged versions,
# removing unused rules (.fw-bold, .fw-bolder, .placeholder variants)
# and reducing container image size for faster AKS deployments
COPY --from=css-purge /css-build/wwwroot/lib/bootstrap/dist/css/ \
     ./wwwroot/lib/bootstrap/dist/css/

# Copy purged site CSS as well
COPY --from=css-purge /css-build/wwwroot/css/ \
     ./wwwroot/css/

# Expose HTTP port
EXPOSE 80
EXPOSE 443

# Set environment variables for containerized deployment
# Override these via AKS ConfigMap / Secret or ACR environment injection
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:80 \
    ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}

# Health check for AKS liveness/readiness probes
# Endpoint: /health (registered in Program.cs via app.MapHealthChecks("/health"))
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "OrbitAOS.V6.dll"]
