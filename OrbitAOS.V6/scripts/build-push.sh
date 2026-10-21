#!/bin/bash
set -e

# =============================================================================
# build-push.sh - Build and Push Docker Image for OrbitAOS.V6
# Target Platform: Azure AKS
# =============================================================================

PROJECT_NAME="orbitaos-v6"
DOCKERFILE_PATH="OrbitAOS.V6/Dockerfile"

echo "=============================================="
echo "  OrbitAOS.V6 - Docker Build & Push Script"
echo "=============================================="
echo ""

# --- Registry Selection ---
echo "Select container registry:"
echo "  1. Azure Container Registry (ACR)"
echo "  2. Docker Hub"
echo ""
read -rp "Enter choice [1-2]: " REGISTRY_CHOICE

# --- Image Tag ---
read -rp "Enter image tag (press Enter for 'latest'): " IMAGE_TAG_INPUT
IMAGE_TAG_INPUT=$(echo "$IMAGE_TAG_INPUT" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
if [ -z "$IMAGE_TAG_INPUT" ]; then
  IMAGE_TAG="latest"
else
  IMAGE_TAG="$IMAGE_TAG_INPUT"
fi

# --- Sanitize image name ---
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo ""
echo "Image name : $IMAGE_NAME"
echo "Image tag  : $IMAGE_TAG"
echo ""

# =============================================================================
# Registry-specific logic
# =============================================================================
if [ "$REGISTRY_CHOICE" = "1" ]; then
  # --- Azure Container Registry ---
  echo "--- Azure Container Registry Setup ---"
  read -rp "Enter ACR name (e.g. myregistry): " ACR_NAME
  ACR_NAME=$(echo "$ACR_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

  FULL_IMAGE_NAME="${ACR_NAME}.azurecr.io/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to ACR: $ACR_NAME ..."
  az acr login --name "$ACR_NAME"
  if [ $? -ne 0 ]; then
    echo "ERROR: ACR login failed. Ensure you are logged in with 'az login'."
    exit 1
  fi

elif [ "$REGISTRY_CHOICE" = "2" ]; then
  # --- Docker Hub ---
  echo "--- Docker Hub Setup ---"
  read -rp "Enter Docker Hub username: " DOCKER_USERNAME
  read -rsp "Enter Docker Hub password/token: " DOCKER_PASSWORD
  echo ""

  FULL_IMAGE_NAME="${DOCKER_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Docker Hub ..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
  if [ $? -ne 0 ]; then
    echo "ERROR: Docker Hub login failed."
    exit 1
  fi

else
  echo "ERROR: Invalid registry choice. Please enter 1 or 2."
  exit 1
fi

echo ""
echo "Full image name: $FULL_IMAGE_NAME"
echo ""

# =============================================================================
# Build Docker Image
# =============================================================================
echo "Building Docker image ..."
echo "  Context    : . (repository root)"
echo "  Dockerfile : $DOCKERFILE_PATH"
echo ""

docker build -f "$DOCKERFILE_PATH" -t "$FULL_IMAGE_NAME" .
if [ $? -ne 0 ]; then
  echo "ERROR: Docker build failed."
  exit 1
fi

echo ""
echo "Docker build succeeded."
echo ""

# =============================================================================
# Push Docker Image
# =============================================================================
echo "Pushing image to registry ..."
docker push "$FULL_IMAGE_NAME"
if [ $? -ne 0 ]; then
  echo "ERROR: Docker push failed."
  exit 1
fi

echo ""
echo "=============================================="
echo "  Image pushed successfully!"
echo "  $FULL_IMAGE_NAME"
echo "=============================================="
echo ""
echo "Next step: Run scripts/deploy-image.sh to deploy to Azure AKS."
