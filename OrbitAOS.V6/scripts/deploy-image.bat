@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: deploy-image.bat - Deploy OrbitAOS.V6 to Azure AKS
:: =============================================================================

set "APP_NAME=orbitaos-v6"
set "NAMESPACE=orbitaos-v6"
set "K8S_DIR=kubernetes"

echo ==============================================
echo   OrbitAOS.V6 - AKS Deployment Script
echo ==============================================
echo.

:: =============================================================================
:: Collect Azure / AKS details
:: =============================================================================
set /p RESOURCE_GROUP="Enter Azure Resource Group name: "
if "!RESOURCE_GROUP!"=="" (
    echo ERROR: Resource group cannot be empty.
    exit /b 1
)

set /p CLUSTER_NAME="Enter AKS Cluster name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: AKS cluster name cannot be empty.
    exit /b 1
)

set /p IMAGE_URI="Enter full Docker image URI (e.g. myregistry.azurecr.io/orbitaos-v6:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Image URI cannot be empty.
    exit /b 1
)

echo.
echo --- Application Environment Variables ---
echo These values will be injected into the Kubernetes deployment.
echo Press Enter to skip any optional variable.
echo.

set /p CONNECTION_STRING="Enter SQL Server connection string (ConnectionStrings__DefaultConnection): "
if "!CONNECTION_STRING!"=="" (
    set "CONNECTION_STRING=Server=^<your-sql-server^>;Database=OrbitAOS;User Id=^<user^>;Password=^<password^>;MultipleActiveResultSets=true"
    echo   Using placeholder value.
)

set /p ASPNETCORE_DOCS_URL="Enter ASP.NET Core Docs URL (ApiUrls__AspNetCoreDocsUrl) [https://docs.microsoft.com/aspnet/core]: "
if "!ASPNETCORE_DOCS_URL!"=="" (
    set "ASPNETCORE_DOCS_URL=https://docs.microsoft.com/aspnet/core"
)

set /p STATIC_FILES_CACHE_MAX_AGE="Enter static files cache max age in seconds (STATIC_FILES_CACHE_MAX_AGE) [86400]: "
if "!STATIC_FILES_CACHE_MAX_AGE!"=="" (
    set "STATIC_FILES_CACHE_MAX_AGE=86400"
)

echo.
echo Summary:
echo   Resource Group : !RESOURCE_GROUP!
echo   AKS Cluster    : !CLUSTER_NAME!
echo   Image URI      : !IMAGE_URI!
echo   Namespace      : !NAMESPACE!
echo.

:: =============================================================================
:: Configure kubectl for AKS
:: =============================================================================
echo Configuring kubectl for AKS cluster ...
az aks get-credentials --resource-group "!RESOURCE_GROUP!" --name "!CLUSTER_NAME!" --overwrite-existing
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to get AKS credentials. Check resource group and cluster name.
    exit /b 1
)

echo Verifying cluster connectivity ...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to AKS cluster.
    exit /b 1
)
echo.

:: =============================================================================
:: Prepare manifest copies in a temp directory
:: =============================================================================
set "DEPLOY_DIR=%TEMP%\orbitaos-v6-deploy-%RANDOM%"
mkdir "!DEPLOY_DIR!"
xcopy /E /I /Q "!K8S_DIR!" "!DEPLOY_DIR!" >nul

echo Substituting placeholders in manifests ...

:: Use PowerShell to perform sed-like replacements
powershell -NoProfile -Command ^
  "(Get-Content '!DEPLOY_DIR!\deployment.yaml') ^
   -replace '\{\{IMAGE_URI\}\}','!IMAGE_URI!' ^
   -replace '\{\{CONNECTION_STRING\}\}','!CONNECTION_STRING!' ^
   -replace '\{\{ASPNETCORE_DOCS_URL\}\}','!ASPNETCORE_DOCS_URL!' ^
   -replace '\{\{STATIC_FILES_CACHE_MAX_AGE\}\}','!STATIC_FILES_CACHE_MAX_AGE!' ^
   | Set-Content '!DEPLOY_DIR!\deployment.yaml'"

if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to substitute placeholders.
    exit /b 1
)

echo Placeholders substituted.
echo.

:: =============================================================================
:: Apply Kubernetes manifests
:: =============================================================================
echo Applying Kubernetes manifests ...

echo   [1/4] Applying namespace ...
kubectl apply -f "!DEPLOY_DIR!\namespace.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. & exit /b 1 )

echo   [2/4] Applying deployment ...
kubectl apply -f "!DEPLOY_DIR!\deployment.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. & exit /b 1 )

echo   [3/4] Applying service ...
kubectl apply -f "!DEPLOY_DIR!\service.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. & exit /b 1 )

echo   [4/4] Applying ingress ...
kubectl apply -f "!DEPLOY_DIR!\ingress.yaml"
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. & exit /b 1 )

echo.

:: =============================================================================
:: Wait for rollout
:: =============================================================================
echo Waiting for deployment rollout ...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed or timed out.
    echo.
    echo To rollback, run:
    echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
    rmdir /s /q "!DEPLOY_DIR!"
    exit /b 1
)

echo.

:: =============================================================================
:: Verify resources
:: =============================================================================
echo Verifying deployed resources ...
kubectl get pods,svc,ingress -n !NAMESPACE!
echo.

:: =============================================================================
:: Display access information
:: =============================================================================
echo ==============================================
echo   Deployment Successful!
echo ==============================================
echo   Ingress Host : http://orbitaos-v6.example.com
echo   Health Check : http://orbitaos-v6.example.com/health
echo   Namespace    : !NAMESPACE!
echo.
echo Useful commands:
echo   kubectl get pods -n !NAMESPACE!
echo   kubectl logs -l app=!APP_NAME! -n !NAMESPACE!
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
echo ==============================================

:: Cleanup
rmdir /s /q "!DEPLOY_DIR!"

endlocal
