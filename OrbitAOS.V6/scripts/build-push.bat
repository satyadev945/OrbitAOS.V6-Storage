@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: build-push.bat - Build and Push Docker Image for OrbitAOS.V6
:: Target Platform: Azure AKS
:: =============================================================================

set "PROJECT_NAME=orbitaos-v6"
set "DOCKERFILE_PATH=OrbitAOS.V6\Dockerfile"

echo ==============================================
echo   OrbitAOS.V6 - Docker Build ^& Push Script
echo ==============================================
echo.

:: --- Registry Selection ---
echo Select container registry:
echo   1. Azure Container Registry (ACR)
echo   2. Docker Hub
echo.
set /p REGISTRY_CHOICE="Enter choice [1-2]: "

:: --- Image Tag ---
set /p IMAGE_TAG_INPUT="Enter image tag (press Enter for 'latest'): "

:: Sanitize tag using PowerShell
for /f "delims=" %%i in ('powershell -NoProfile -Command "$t = '%IMAGE_TAG_INPUT%'.ToLower() -replace '[^a-z0-9._-]','-'; $t = $t.Trim('-'); if ($t -eq '') { 'latest' } else { $t }"') do set "IMAGE_TAG=%%i"

:: Sanitize image name
for /f "delims=" %%i in ('powershell -NoProfile -Command "$n = '%PROJECT_NAME%'.ToLower() -replace '[^a-z0-9]','-'; $n.Trim('-')"') do set "IMAGE_NAME=%%i"

echo.
echo Image name : !IMAGE_NAME!
echo Image tag  : !IMAGE_TAG!
echo.

:: =============================================================================
:: Registry-specific logic
:: =============================================================================
if "!REGISTRY_CHOICE!"=="1" (
    :: --- Azure Container Registry ---
    echo --- Azure Container Registry Setup ---
    set /p ACR_NAME="Enter ACR name (e.g. myregistry): "

    for /f "delims=" %%i in ('powershell -NoProfile -Command "$n = '!ACR_NAME!'.ToLower() -replace '[^a-z0-9]','-'; $n.Trim('-')"') do set "ACR_NAME_CLEAN=%%i"

    set "FULL_IMAGE_NAME=!ACR_NAME_CLEAN!.azurecr.io/!IMAGE_NAME!:!IMAGE_TAG!"

    echo.
    echo Logging in to ACR: !ACR_NAME_CLEAN! ...
    az acr login --name !ACR_NAME_CLEAN!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: ACR login failed. Ensure you are logged in with 'az login'.
        exit /b 1
    )

) else if "!REGISTRY_CHOICE!"=="2" (
    :: --- Docker Hub ---
    echo --- Docker Hub Setup ---
    set /p DOCKER_USERNAME="Enter Docker Hub username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub password/token: "

    set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!:!IMAGE_TAG!"

    echo.
    echo Logging in to Docker Hub ...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Docker Hub login failed.
        exit /b 1
    )

) else (
    echo ERROR: Invalid registry choice. Please enter 1 or 2.
    exit /b 1
)

echo.
echo Full image name: !FULL_IMAGE_NAME!
echo.

:: =============================================================================
:: Build Docker Image
:: =============================================================================
echo Building Docker image ...
echo   Context    : . (repository root)
echo   Dockerfile : !DOCKERFILE_PATH!
echo.

docker build -f "!DOCKERFILE_PATH!" -t "!FULL_IMAGE_NAME!" .
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)

echo.
echo Docker build succeeded.
echo.

:: =============================================================================
:: Push Docker Image
:: =============================================================================
echo Pushing image to registry ...
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo   Image pushed successfully!
echo   !FULL_IMAGE_NAME!
echo ==============================================
echo.
echo Next step: Run scripts\deploy-image.bat to deploy to Azure AKS.

endlocal
