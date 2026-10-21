# OrbitAOS.V6 - Deployment Guide

## Overview

This guide covers building, containerizing, and deploying the **OrbitAOS.V6** ASP.NET Core 6.0 web application to **Azure Kubernetes Service (AKS)**.

- **Application**: OrbitAOS.V6
- **Framework**: ASP.NET Core 6.0 (MVC + Identity + Entity Framework Core)
- **Runtime Image**: `mcr.microsoft.com/dotnet/runtime:6.0-alpine`
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:6.0`
- **Target Platform**: Azure AKS
- **Application Port**: 80 (HTTP)
- **Health Endpoint**: `/health`

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Structure](#project-structure)
3. [Local Development with Docker Compose](#local-development-with-docker-compose)
4. [Build and Push Docker Image](#build-and-push-docker-image)
5. [Azure AKS Prerequisites](#azure-aks-prerequisites)
6. [AKS Cluster Setup](#aks-cluster-setup)
7. [Kubernetes Deployment](#kubernetes-deployment)
8. [Configuration Management](#configuration-management)
9. [Scaling and Management](#scaling-and-management)
10. [Troubleshooting](#troubleshooting)
11. [Security Considerations](#security-considerations)
12. [.NET-Specific Notes](#net-specific-notes)

---

## Prerequisites

### Local Development
- [Docker Desktop](https://www.docker.com/products/docker-desktop) 20.10+
- [Docker Compose](https://docs.docker.com/compose/install/) v2+
- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

### Azure AKS Deployment
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) 2.40+
- [kubectl](https://kubernetes.io/docs/tasks/tools/) 1.25+
- Active Azure subscription
- Azure Container Registry (ACR) or Docker Hub account
- AKS cluster with Application Gateway Ingress Controller (AGIC) enabled

---

## Project Structure

```
OrbitAOS.V6/
├── Dockerfile                    # Multi-stage Docker build
├── docker-compose.yml            # Local development compose file
├── .dockerignore                 # Docker build exclusions
├── kubernetes/
│   ├── namespace.yaml            # Kubernetes namespace
│   ├── deployment.yaml           # Application deployment
│   ├── service.yaml              # ClusterIP service
│   └── ingress.yaml              # Azure Application Gateway ingress
├── scripts/
│   ├── build-push.sh             # Linux/macOS build & push script
│   ├── build-push.bat            # Windows build & push script
│   ├── deploy-image.sh           # Linux/macOS AKS deploy script
│   └── deploy-image.bat          # Windows AKS deploy script
└── docs/
    └── DEPLOYMENT.md             # This file
```

---

## Local Development with Docker Compose

### 1. Configure Environment Variables

Create a `.env` file in the project root (never commit this file):

```env
CONNECTION_STRING=Server=<your-sql-server>;Database=OrbitAOS;User Id=<user>;Password=<password>;MultipleActiveResultSets=true
ASPNETCORE_DOCS_URL=https://docs.microsoft.com/aspnet/core
STATIC_FILES_CACHE_MAX_AGE=86400
```

### 2. Start the Application

```bash
# From the OrbitAOS.V6 directory
docker-compose up --build
```

### 3. Access the Application

- Application: [http://localhost:8080](http://localhost:8080)
- Health Check: [http://localhost:8080/health](http://localhost:8080/health)

### 4. Stop the Application

```bash
docker-compose down
```

---

## Build and Push Docker Image

### Linux / macOS

```bash
# Make the script executable
chmod +x scripts/build-push.sh

# Run from the OrbitAOS.V6 directory
./scripts/build-push.sh
```

The script will prompt you to:
1. Select registry type (Azure ACR or Docker Hub)
2. Enter registry credentials
3. Enter an image tag (defaults to `latest`)

### Windows

```cmd
scripts\build-push.bat
```

### Manual Build (Advanced)

```bash
# Build from repository root
docker build -f OrbitAOS.V6/Dockerfile -t <registry>/orbitaos-v6:latest .

# Push to registry
docker push <registry>/orbitaos-v6:latest
```

---

## Azure AKS Prerequisites

### 1. Install Azure CLI

```bash
# macOS
brew install azure-cli

# Linux
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Windows
winget install Microsoft.AzureCLI
```

### 2. Install kubectl

```bash
az aks install-cli
```

### 3. Login to Azure

```bash
az login
az account set --subscription "<your-subscription-id>"
```

### 4. Create Azure Container Registry (if not existing)

```bash
az acr create \
  --resource-group <your-resource-group> \
  --name <your-acr-name> \
  --sku Basic

# Login to ACR
az acr login --name <your-acr-name>
```

---

## AKS Cluster Setup

### 1. Create AKS Cluster (if not existing)

```bash
az aks create \
  --resource-group <your-resource-group> \
  --name <your-cluster-name> \
  --node-count 2 \
  --node-vm-size Standard_DS2_v2 \
  --enable-addons monitoring \
  --generate-ssh-keys \
  --attach-acr <your-acr-name>
```

### 2. Enable Application Gateway Ingress Controller

```bash
az aks enable-addons \
  --resource-group <your-resource-group> \
  --name <your-cluster-name> \
  --addons ingress-appgw \
  --appgw-name <your-appgw-name> \
  --appgw-subnet-cidr "10.225.0.0/16"
```

### 3. Configure kubectl

```bash
az aks get-credentials \
  --resource-group <your-resource-group> \
  --name <your-cluster-name>

# Verify connection
kubectl cluster-info
kubectl get nodes
```

---

## Kubernetes Deployment

### Automated Deployment (Recommended)

#### Linux / macOS

```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

#### Windows

```cmd
scripts\deploy-image.bat
```

The script will prompt for:
- Azure Resource Group name
- AKS Cluster name
- Full Docker image URI (e.g., `myregistry.azurecr.io/orbitaos-v6:latest`)
- SQL Server connection string
- Other environment variables

### Manual Deployment

#### 1. Apply Namespace

```bash
kubectl apply -f kubernetes/namespace.yaml
```

#### 2. Update Deployment Manifest

Edit `kubernetes/deployment.yaml` and replace placeholders:
- `{{IMAGE_URI}}` → your full image URI
- `{{CONNECTION_STRING}}` → your SQL Server connection string
- `{{ASPNETCORE_DOCS_URL}}` → API docs URL
- `{{STATIC_FILES_CACHE_MAX_AGE}}` → cache max age in seconds

#### 3. Apply Manifests

```bash
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml
```

#### 4. Verify Deployment

```bash
kubectl get pods -n orbitaos-v6
kubectl get svc -n orbitaos-v6
kubectl get ingress -n orbitaos-v6
```

#### 5. Check Rollout Status

```bash
kubectl rollout status deployment/orbitaos-v6 -n orbitaos-v6
```

---

## Configuration Management

### Environment Variables

| Variable | Description | Default |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production` |
| `ASPNETCORE_URLS` | Kestrel binding URL | `http://+:80` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | Required |
| `ApiUrls__AspNetCoreDocsUrl` | ASP.NET Core docs URL | `https://docs.microsoft.com/aspnet/core` |
| `STATIC_FILES_CACHE_MAX_AGE` | Static file cache duration (seconds) | `86400` |
| `DOTNET_RUNNING_IN_CONTAINER` | Container detection flag | `true` |

### Using Kubernetes Secrets (Recommended for Production)

```bash
# Create secret for connection string
kubectl create secret generic orbitaos-v6-secrets \
  --from-literal=connection-string="Server=<server>;Database=OrbitAOS;..." \
  -n orbitaos-v6
```

Then reference in `deployment.yaml`:

```yaml
env:
  - name: ConnectionStrings__DefaultConnection
    valueFrom:
      secretKeyRef:
        name: orbitaos-v6-secrets
        key: connection-string
```

---

## Scaling and Management

### Manual Scaling

```bash
kubectl scale deployment orbitaos-v6 --replicas=3 -n orbitaos-v6
```

### Horizontal Pod Autoscaler (HPA)

```bash
kubectl autoscale deployment orbitaos-v6 \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n orbitaos-v6
```

### Rolling Update

```bash
# Update image
kubectl set image deployment/orbitaos-v6 \
  orbitaos-v6=<registry>/orbitaos-v6:<new-tag> \
  -n orbitaos-v6

# Monitor rollout
kubectl rollout status deployment/orbitaos-v6 -n orbitaos-v6
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/orbitaos-v6 -n orbitaos-v6

# Rollback to specific revision
kubectl rollout undo deployment/orbitaos-v6 --to-revision=2 -n orbitaos-v6

# View rollout history
kubectl rollout history deployment/orbitaos-v6 -n orbitaos-v6
```

---

## Troubleshooting

### Pod Not Starting

```bash
# Check pod status
kubectl get pods -n orbitaos-v6

# Describe pod for events
kubectl describe pod <pod-name> -n orbitaos-v6

# View pod logs
kubectl logs <pod-name> -n orbitaos-v6

# View previous container logs (if crashed)
kubectl logs <pod-name> -n orbitaos-v6 --previous
```

### Health Check Failures

```bash
# Test health endpoint from within the cluster
kubectl exec -it <pod-name> -n orbitaos-v6 -- wget -qO- http://localhost:80/health

# Check liveness/readiness probe events
kubectl describe pod <pod-name> -n orbitaos-v6 | grep -A 10 "Liveness\|Readiness"
```

### Database Connection Issues

```bash
# Verify connection string secret
kubectl get secret orbitaos-v6-secrets -n orbitaos-v6 -o yaml

# Check application logs for EF Core errors
kubectl logs -l app=orbitaos-v6 -n orbitaos-v6 | grep -i "error\|exception\|connection"
```

### Ingress Not Accessible

```bash
# Check ingress status
kubectl describe ingress orbitaos-v6-ingress -n orbitaos-v6

# Verify Application Gateway is provisioned
az network application-gateway show \
  --resource-group <your-resource-group> \
  --name <your-appgw-name>
```

### Image Pull Errors

```bash
# Verify ACR integration with AKS
az aks check-acr \
  --resource-group <your-resource-group> \
  --name <your-cluster-name> \
  --acr <your-acr-name>

# Attach ACR to AKS if needed
az aks update \
  --resource-group <your-resource-group> \
  --name <your-cluster-name> \
  --attach-acr <your-acr-name>
```

---

## Security Considerations

1. **Non-root container**: The Dockerfile creates and uses a non-root user (`appuser`) for running the application.
2. **Secrets management**: Use Kubernetes Secrets or Azure Key Vault for sensitive configuration (connection strings, API keys).
3. **Network policies**: Consider adding Kubernetes NetworkPolicy resources to restrict pod-to-pod communication.
4. **HTTPS**: Configure TLS termination at the Application Gateway level for production deployments.
5. **Image scanning**: Enable Azure Defender for container registries to scan images for vulnerabilities.
6. **RBAC**: Use Kubernetes RBAC to restrict access to the namespace.
7. **Resource limits**: CPU and memory limits are configured to prevent resource exhaustion.
8. **Read-only filesystem**: Consider adding `readOnlyRootFilesystem: true` to the security context where possible.

---

## .NET-Specific Notes

### Entity Framework Core Migrations

Run database migrations before or during deployment:

```bash
# Option 1: Run migrations from local machine
dotnet ef database update --connection "<connection-string>"

# Option 2: Run migrations as a Kubernetes Job (recommended for production)
# Create a migration job that runs before the main deployment
```

### ASP.NET Core Identity

The application uses ASP.NET Core Identity with SQL Server. Ensure:
- The database is accessible from the AKS cluster
- The connection string has appropriate permissions (CREATE TABLE, ALTER TABLE)
- Initial migrations are applied before first startup

### Static Files Caching

The application configures static file caching via the `STATIC_FILES_CACHE_MAX_AGE` environment variable. For production:
- Set to `86400` (1 day) or higher for immutable assets
- Consider using Azure CDN in front of the Application Gateway for better performance

### Kestrel Configuration

The application uses Kestrel (the built-in ASP.NET Core web server) bound to `http://+:80`. HTTPS termination is handled at the Application Gateway level.

### Logging

Logs are written to stdout/stderr and can be viewed with:

```bash
kubectl logs -l app=orbitaos-v6 -n orbitaos-v6 --follow
```

For production, consider integrating with Azure Monitor / Application Insights by adding the `Microsoft.ApplicationInsights.AspNetCore` NuGet package.

### Performance Tuning

- The runtime image uses `mcr.microsoft.com/dotnet/runtime:6.0-alpine` for minimal image size
- Consider setting `DOTNET_GCConserveMemory=5` for memory-constrained environments
- Use `ReadyToRun` compilation for faster startup: add `<PublishReadyToRun>true</PublishReadyToRun>` to the `.csproj`

---

*Generated for OrbitAOS.V6 | ASP.NET Core 6.0 | Azure AKS | Target: Production*
