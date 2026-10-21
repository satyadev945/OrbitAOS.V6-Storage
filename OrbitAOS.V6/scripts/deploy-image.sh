#!/bin/bash
set -e
set -o pipefail

# =============================================================================
# deploy-image.sh - Deploy OrbitAOS.V6 to Azure AKS
# =============================================================================

APP_NAME="orbitaos-v6"
NAMESPACE="orbitaos-v6"
K8S_DIR="kubernetes"

echo "=============================================="
echo "  OrbitAOS.V6 - AKS Deployment Script"
echo "=============================================="
echo ""

# =============================================================================
# Collect Azure / AKS details
# =============================================================================
read -rp "Enter Azure Resource Group name: " RESOURCE_GROUP
if [ -z "$RESOURCE_GROUP" ]; then
  echo "ERROR: Resource group cannot be empty."
  exit 1
fi

read -rp "Enter AKS Cluster name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: AKS cluster name cannot be empty."
  exit 1
fi

read -rp "Enter full Docker image URI (e.g. myregistry.azurecr.io/orbitaos-v6:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Image URI cannot be empty."
  exit 1
fi

echo ""
echo "--- Application Environment Variables ---"
echo "These values will be injected into the Kubernetes deployment."
echo "Press Enter to skip any optional variable."
echo ""

read -rp "Enter SQL Server connection string (ConnectionStrings__DefaultConnection): " CONNECTION_STRING
if [ -z "$CONNECTION_STRING" ]; then
  CONNECTION_STRING="Server=<your-sql-server>;Database=OrbitAOS;User Id=<user>;Password=<password>;MultipleActiveResultSets=true"
  echo "  Using placeholder: $CONNECTION_STRING"
fi

read -rp "Enter ASP.NET Core Docs URL (ApiUrls__AspNetCoreDocsUrl) [https://docs.microsoft.com/aspnet/core]: " ASPNETCORE_DOCS_URL
if [ -z "$ASPNETCORE_DOCS_URL" ]; then
  ASPNETCORE_DOCS_URL="https://docs.microsoft.com/aspnet/core"
fi

read -rp "Enter static files cache max age in seconds (STATIC_FILES_CACHE_MAX_AGE) [86400]: " STATIC_FILES_CACHE_MAX_AGE
if [ -z "$STATIC_FILES_CACHE_MAX_AGE" ]; then
  STATIC_FILES_CACHE_MAX_AGE="86400"
fi

echo ""
echo "Summary:"
echo "  Resource Group : $RESOURCE_GROUP"
echo "  AKS Cluster    : $CLUSTER_NAME"
echo "  Image URI      : $IMAGE_URI"
echo "  Namespace      : $NAMESPACE"
echo ""

# =============================================================================
# Configure kubectl for AKS
# =============================================================================
echo "Configuring kubectl for AKS cluster ..."
az aks get-credentials --resource-group "$RESOURCE_GROUP" --name "$CLUSTER_NAME" --overwrite-existing
if [ $? -ne 0 ]; then
  echo "ERROR: Failed to get AKS credentials. Check resource group and cluster name."
  exit 1
fi

echo "Verifying cluster connectivity ..."
kubectl cluster-info || { echo "ERROR: Cannot connect to AKS cluster."; exit 1; }
echo ""

# =============================================================================
# Prepare manifest copies (avoid modifying originals)
# =============================================================================
DEPLOY_DIR=$(mktemp -d)
cp -r "$K8S_DIR/"* "$DEPLOY_DIR/"

echo "Substituting placeholders in manifests ..."

# Replace IMAGE_URI
sed -i "s|{{IMAGE_URI}}|${IMAGE_URI}|g" "$DEPLOY_DIR/deployment.yaml"

# Replace environment variable placeholders
sed -i "s|{{CONNECTION_STRING}}|${CONNECTION_STRING}|g"                   "$DEPLOY_DIR/deployment.yaml"
sed -i "s|{{ASPNETCORE_DOCS_URL}}|${ASPNETCORE_DOCS_URL}|g"               "$DEPLOY_DIR/deployment.yaml"
sed -i "s|{{STATIC_FILES_CACHE_MAX_AGE}}|${STATIC_FILES_CACHE_MAX_AGE}|g" "$DEPLOY_DIR/deployment.yaml"

echo "Placeholders substituted."
echo ""

# =============================================================================
# Apply Kubernetes manifests
# =============================================================================
echo "Applying Kubernetes manifests ..."

echo "  [1/4] Applying namespace ..."
kubectl apply -f "$DEPLOY_DIR/namespace.yaml"

echo "  [2/4] Applying deployment ..."
kubectl apply -f "$DEPLOY_DIR/deployment.yaml"

echo "  [3/4] Applying service ..."
kubectl apply -f "$DEPLOY_DIR/service.yaml"

echo "  [4/4] Applying ingress ..."
kubectl apply -f "$DEPLOY_DIR/ingress.yaml"

echo ""

# =============================================================================
# Wait for rollout
# =============================================================================
echo "Waiting for deployment rollout ..."
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s
if [ $? -ne 0 ]; then
  echo "ERROR: Deployment rollout failed or timed out."
  echo ""
  echo "To rollback, run:"
  echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
  rm -rf "$DEPLOY_DIR"
  exit 1
fi

echo ""

# =============================================================================
# Verify resources
# =============================================================================
echo "Verifying deployed resources ..."
kubectl get pods,svc,ingress -n "$NAMESPACE"
echo ""

# =============================================================================
# Display access URL
# =============================================================================
INGRESS_IP=$(kubectl get ingress "${APP_NAME}-ingress" -n "$NAMESPACE" -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null || echo "")
INGRESS_HOST=$(kubectl get ingress "${APP_NAME}-ingress" -n "$NAMESPACE" -o jsonpath='{.spec.rules[0].host}' 2>/dev/null || echo "orbitaos-v6.example.com")

echo "=============================================="
echo "  Deployment Successful!"
echo "=============================================="
if [ -n "$INGRESS_IP" ]; then
  echo "  Application URL : http://$INGRESS_IP"
fi
echo "  Ingress Host    : http://$INGRESS_HOST"
echo "  Health Check    : http://$INGRESS_HOST/health"
echo "  Namespace       : $NAMESPACE"
echo ""
echo "Useful commands:"
echo "  kubectl get pods -n $NAMESPACE"
echo "  kubectl logs -l app=$APP_NAME -n $NAMESPACE"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE  # rollback"
echo "=============================================="

# Cleanup temp dir
rm -rf "$DEPLOY_DIR"
