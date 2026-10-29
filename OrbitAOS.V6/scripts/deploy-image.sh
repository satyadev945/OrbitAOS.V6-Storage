#!/bin/bash
set -e
set -o pipefail

# Deploy to AWS ECS Fargate - OrbitAOS.V6
# This script deploys the containerized application to AWS ECS Fargate

echo "=========================================="
echo "OrbitAOS.V6 - AWS ECS Fargate Deployment"
echo "=========================================="
echo ""

# Configuration
PROJECT_NAME="orbitaos-v6"
TASK_FAMILY="orbitaos-v6-task"
SERVICE_NAME="orbitaos-v6-service"
CONTAINER_NAME="orbitaos-v6"

# Prompt for AWS configuration
echo "=== AWS Configuration ==="
read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS Cluster Name (e.g., my-ecs-cluster): " CLUSTER_NAME

# Get AWS Account ID
echo ""
echo "Retrieving AWS Account ID..."
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "AWS Account ID: $ACCOUNT_ID"

# Check if cluster exists, create if not
echo ""
echo "Checking if ECS cluster exists..."
aws ecs describe-clusters --clusters "$CLUSTER_NAME" --region "$AWS_REGION" >/dev/null 2>&1 || {
    echo "Cluster does not exist. Creating ECS cluster: $CLUSTER_NAME"
    aws ecs create-cluster --cluster-name "$CLUSTER_NAME" --region "$AWS_REGION"
    echo "ECS cluster created successfully"
}

# Prompt for network configuration
echo ""
echo "=== Network Configuration ==="
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNET_IDS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP

# Parse subnet IDs
IFS=',' read -ra SUBNET_ARRAY <<< "$SUBNET_IDS"
SUBNET_1="${SUBNET_ARRAY[0]}"
SUBNET_2="${SUBNET_ARRAY[1]:-$SUBNET_1}"

# Prompt for Docker image
echo ""
echo "=== Docker Image Configuration ==="
read -p "Enter Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/orbitaos-v6:latest): " IMAGE_URI

# Prompt for database connection string
echo ""
echo "=== Database Configuration ==="
read -p "Enter Database Connection String: " DB_CONNECTION_STRING

# Load balancer configuration
echo ""
echo "=== Load Balancer Configuration ==="
read -p "Do you need a load balancer for this service? (y/n): " NEED_LB

if [[ "$NEED_LB" =~ ^[Yy]$ ]]; then
    echo ""
    echo "Creating Application Load Balancer and Target Group..."
    
    # Create target group with ip target type (required for Fargate awsvpc)
    TG_NAME="orbitaos-v6-tg"
    TARGET_GROUP_ARN=$(aws elbv2 create-target-group \
        --name "$TG_NAME" \
        --protocol HTTP \
        --port 80 \
        --vpc-id "$VPC_ID" \
        --target-type ip \
        --health-check-enabled \
        --health-check-path "/health" \
        --health-check-interval-seconds 30 \
        --health-check-timeout-seconds 5 \
        --healthy-threshold-count 2 \
        --unhealthy-threshold-count 3 \
        --region "$AWS_REGION" \
        --query 'TargetGroups[0].TargetGroupArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        # Target group might already exist, try to describe it
        TARGET_GROUP_ARN=$(aws elbv2 describe-target-groups \
            --names "$TG_NAME" \
            --region "$AWS_REGION" \
            --query 'TargetGroups[0].TargetGroupArn' \
            --output text 2>/dev/null || echo "")
    fi
    
    if [ -z "$TARGET_GROUP_ARN" ]; then
        echo "ERROR: Failed to create or find target group"
        exit 1
    fi
    
    echo "Target Group ARN: $TARGET_GROUP_ARN"
    
    # Create Application Load Balancer
    ALB_NAME="orbitaos-v6-alb"
    ALB_ARN=$(aws elbv2 create-load-balancer \
        --name "$ALB_NAME" \
        --subnets $SUBNET_1 $SUBNET_2 \
        --security-groups "$SECURITY_GROUP" \
        --scheme internet-facing \
        --type application \
        --ip-address-type ipv4 \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].LoadBalancerArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$ALB_ARN" ]; then
        # ALB might already exist, try to describe it
        ALB_ARN=$(aws elbv2 describe-load-balancers \
            --names "$ALB_NAME" \
            --region "$AWS_REGION" \
            --query 'LoadBalancers[0].LoadBalancerArn' \
            --output text 2>/dev/null || echo "")
    fi
    
    if [ -z "$ALB_ARN" ]; then
        echo "ERROR: Failed to create or find load balancer"
        exit 1
    fi
    
    echo "Load Balancer ARN: $ALB_ARN"
    
    # Create listener
    LISTENER_ARN=$(aws elbv2 create-listener \
        --load-balancer-arn "$ALB_ARN" \
        --protocol HTTP \
        --port 80 \
        --default-actions Type=forward,TargetGroupArn="$TARGET_GROUP_ARN" \
        --region "$AWS_REGION" \
        --query 'Listeners[0].ListenerArn' \
        --output text 2>/dev/null || echo "")
    
    if [ -z "$LISTENER_ARN" ]; then
        echo "WARNING: Listener might already exist or failed to create"
    else
        echo "Listener created successfully"
    fi
    
    # Get ALB DNS name
    ALB_DNS=$(aws elbv2 describe-load-balancers \
        --load-balancer-arns "$ALB_ARN" \
        --region "$AWS_REGION" \
        --query 'LoadBalancers[0].DNSName' \
        --output text)
    
    echo "Load Balancer DNS: $ALB_DNS"
else
    echo "Skipping load balancer creation"
    TARGET_GROUP_ARN=""
fi

# Create CloudWatch log group
echo ""
echo "Creating CloudWatch log group..."
aws logs create-log-group --log-group-name "/ecs/$PROJECT_NAME" --region "$AWS_REGION" 2>/dev/null || echo "Log group already exists"

# Replace placeholders in task definition
echo ""
echo "Preparing task definition..."
TASK_DEF_FILE="ecs/task-definition.json"
TASK_DEF_TEMP="ecs/task-definition-temp.json"

cp "$TASK_DEF_FILE" "$TASK_DEF_TEMP"

sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" "$TASK_DEF_TEMP"
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" "$TASK_DEF_TEMP"
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" "$TASK_DEF_TEMP"
sed -i "s|{{DB_CONNECTION_STRING}}|$DB_CONNECTION_STRING|g" "$TASK_DEF_TEMP"

# Register task definition
echo "Registering task definition..."
TASK_DEF_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://"$TASK_DEF_TEMP" \
    --region "$AWS_REGION" \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

if [ -z "$TASK_DEF_ARN" ]; then
    echo "ERROR: Failed to register task definition"
    rm -f "$TASK_DEF_TEMP"
    exit 1
fi

echo "Task definition registered: $TASK_DEF_ARN"
rm -f "$TASK_DEF_TEMP"

# Prepare service definition
echo ""
echo "Preparing service definition..."
SERVICE_DEF_FILE="ecs/service-definition.json"
SERVICE_DEF_TEMP="ecs/service-definition-temp.json"

cp "$SERVICE_DEF_FILE" "$SERVICE_DEF_TEMP"

sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{SUBNET_1}}|$SUBNET_1|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{SUBNET_2}}|$SUBNET_2|g" "$SERVICE_DEF_TEMP"
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" "$SERVICE_DEF_TEMP"

if [ -z "$TARGET_GROUP_ARN" ]; then
    # Remove loadBalancers section if no load balancer
    jq 'del(.loadBalancers) | del(.healthCheckGracePeriodSeconds)' "$SERVICE_DEF_TEMP" > "$SERVICE_DEF_TEMP.tmp"
    mv "$SERVICE_DEF_TEMP.tmp" "$SERVICE_DEF_TEMP"
else
    sed -i "s|{{TARGET_GROUP_ARN}}|$TARGET_GROUP_ARN|g" "$SERVICE_DEF_TEMP"
fi

# Check if service exists
echo ""
echo "Checking if service exists..."
EXISTING_SERVICE=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[?status==`ACTIVE`].serviceName' \
    --output text 2>/dev/null || echo "")

if [ -z "$EXISTING_SERVICE" ] || [ "$EXISTING_SERVICE" == "None" ]; then
    # Create new service
    echo "Creating new ECS service..."
    aws ecs create-service \
        --cli-input-json file://"$SERVICE_DEF_TEMP" \
        --region "$AWS_REGION"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to create service"
        rm -f "$SERVICE_DEF_TEMP"
        exit 1
    fi
    
    echo "Service created successfully"
else
    # Update existing service
    echo "Updating existing ECS service..."
    aws ecs update-service \
        --cluster "$CLUSTER_NAME" \
        --service "$SERVICE_NAME" \
        --task-definition "$TASK_DEF_ARN" \
        --desired-count 2 \
        --force-new-deployment \
        --region "$AWS_REGION"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Failed to update service"
        rm -f "$SERVICE_DEF_TEMP"
        exit 1
    fi
    
    echo "Service updated successfully"
fi

rm -f "$SERVICE_DEF_TEMP"

# Wait for service to stabilize
echo ""
echo "Waiting for service to stabilize (this may take several minutes)..."
aws ecs wait services-stable \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION"

if [ $? -eq 0 ]; then
    echo "Service is stable"
else
    echo "WARNING: Service stabilization timed out or failed"
fi

# Display deployment information
echo ""
echo "=========================================="
echo "Deployment Summary"
echo "=========================================="
echo "Cluster: $CLUSTER_NAME"
echo "Service: $SERVICE_NAME"
echo "Task Definition: $TASK_DEF_ARN"
echo "Region: $AWS_REGION"

if [ -n "$ALB_DNS" ]; then
    echo "Load Balancer DNS: http://$ALB_DNS"
    echo "Health Check Endpoint: http://$ALB_DNS/health"
fi

echo "CloudWatch Logs: /ecs/$PROJECT_NAME"
echo ""

# Verify deployment
echo "Verifying deployment..."
RUNNING_COUNT=$(aws ecs describe-services \
    --cluster "$CLUSTER_NAME" \
    --services "$SERVICE_NAME" \
    --region "$AWS_REGION" \
    --query 'services[0].runningCount' \
    --output text)

echo "Running tasks: $RUNNING_COUNT"
echo ""
echo "=========================================="
echo "Deployment completed successfully!"
echo "=========================================="
echo ""
echo "Troubleshooting:"
echo "- View logs: aws logs tail /ecs/$PROJECT_NAME --follow --region $AWS_REGION"
echo "- Describe service: aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --region $AWS_REGION"
echo "- List tasks: aws ecs list-tasks --cluster $CLUSTER_NAME --service-name $SERVICE_NAME --region $AWS_REGION"
echo ""
