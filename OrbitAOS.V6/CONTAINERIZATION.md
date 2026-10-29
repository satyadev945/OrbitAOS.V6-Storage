# Containerization Configuration Guide

## Overview
This document describes the containerization changes made to OrbitAOS.V6 for AWS ECS Fargate deployment.

## Changes Made

### 1. Hardcoded URL Remediation (Rule: cz-dotnet-1002)

**File**: `Views/Home/Index.cshtml`
**Issue**: Hardcoded environment-specific URL prevented container image promotion across environments.

**Before**:
```cshtml
<p>Learn about <a href="https://docs.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
```

**After**:
```cshtml
@inject Microsoft.Extensions.Configuration.IConfiguration Configuration
<p>Learn about <a href="@Configuration["ExternalUrls:AspNetCoreDocsUrl"]">building Web apps with ASP.NET Core</a>.</p>
```

**Configuration** (`appsettings.json`):
```json
{
  "ExternalUrls": {
    "AspNetCoreDocsUrl": "https://docs.microsoft.com/aspnet/core"
  }
}
```

### 2. Health Check Endpoint

**File**: `Program.cs`
**Purpose**: Container orchestration requires health check endpoints for liveness and readiness probes.

**Changes**:
- Added `builder.Services.AddHealthChecks()` to register health check services
- Added `app.MapHealthChecks("/health")` to expose health endpoint at `/health`

**Endpoint**: `GET /health`
**Response**: Returns HTTP 200 with "Healthy" status when application is running correctly.

## AWS ECS Fargate Configuration

### Environment Variables
Configure the following environment variables in your ECS Task Definition:

```json
{
  "environment": [
    {
      "name": "ExternalUrls__AspNetCoreDocsUrl",
      "value": "https://docs.microsoft.com/aspnet/core"
    }
  ]
}
```

### Secrets Management
For sensitive URLs or API endpoints with credentials, use AWS Secrets Manager:

```json
{
  "secrets": [
    {
      "name": "ExternalUrls__ApiUrl",
      "valueFrom": "arn:aws:secretsmanager:region:account-id:secret:api-url-secret"
    }
  ]
}
```

### Health Check Configuration
Configure ECS health check to use the `/health` endpoint:

```json
{
  "healthCheck": {
    "command": ["CMD-SHELL", "curl -f http://localhost/health || exit 1"],
    "interval": 30,
    "timeout": 5,
    "retries": 3,
    "startPeriod": 60
  }
}
```

## Container Image Promotion
With these changes, a single container image can be promoted across environments (dev, staging, production) by:
1. Building the image once
2. Configuring environment-specific values via ECS Task Definition environment variables
3. Using AWS Secrets Manager for sensitive configuration

## Testing Health Check
```bash
# Local testing
curl http://localhost:5000/health

# Expected response
Healthy
```

## Notes
- The health check endpoint is automatically available and requires no authentication
- Configuration values can be overridden using environment variables with double underscore notation (e.g., `ExternalUrls__AspNetCoreDocsUrl`)
- For production deployments, ensure all external URLs are configured via environment variables or AWS Secrets Manager
