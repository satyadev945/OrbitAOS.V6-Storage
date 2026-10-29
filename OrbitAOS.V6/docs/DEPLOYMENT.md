# OrbitAOS.V6 - AWS ECS Fargate Deployment Guide

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Project Architecture](#project-architecture)
4. [Local Development](#local-development)
5. [Docker Containerization](#docker-containerization)
6. [AWS ECS Fargate Deployment](#aws-ecs-fargate-deployment)
7. [Configuration Management](#configuration-management)
8. [Monitoring and Logging](#monitoring-and-logging)
9. [Troubleshooting](#troubleshooting)
10. [Security Considerations](#security-considerations)
11. [Scaling and Performance](#scaling-and-performance)

---

## Overview

OrbitAOS.V6 is an ASP.NET Core 6.0 web application with Identity authentication and Entity Framework Core for data access. This guide provides comprehensive instructions for containerizing and deploying the application to AWS ECS Fargate.

### Technology Stack
- **Framework**: .NET 6.0
- **Application Type**: ASP.NET Core MVC with Identity
- **Database**: SQL Server (via Entity Framework Core)
- **Authentication**: ASP.NET Core Identity
- **Container Platform**: Docker
- **Deployment Target**: AWS ECS Fargate

### Key Features
- ASP.NET Core Identity for user authentication
- Entity Framework Core with SQL Server
- Health check endpoint at `/health`
- Production-ready containerization
- AWS ECS Fargate optimized deployment

---

## Prerequisites

### Required Software
1. **Docker Desktop** (version 20.10 or later)
   - Download: https://www.docker.com/products/docker-desktop
   - Verify: `docker --version`

2. **AWS CLI** (version 2.x)
   - Download: https://aws.amazon.com/cli/
   - Verify: `aws --version`
   - Configure: `aws configure`

3. **.NET 6.0 SDK** (for local development)
   - Download: https://dotnet.microsoft.com/download/dotnet/6.0
   - Verify: `dotnet --version`

### AWS Account Requirements
1. **IAM User** with the following permissions:
   - `AmazonECS_FullAccess`
   - `AmazonEC2ContainerRegistryFullAccess`
   - `CloudWatchLogsFullAccess`
   - `IAMReadOnlyAccess`
   - `ElasticLoadBalancingFullAccess`

2. **AWS Resources**:
   - VPC with at least 2 subnets in different availability zones
   - Security group allowing inbound traffic on port 80
   - SQL Server database (RDS or external)

3. **IAM Roles**:
   - **ecsTaskExecutionRole**: Allows ECS to pull images and write logs
   - **ecsTaskRole**: Allows tasks to access AWS services (optional)

### Create IAM Roles

#### ECS Task Execution Role
```bash
aws iam create-role \
  --role-name ecsTaskExecutionRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'

aws iam attach-role-policy \
  --role-name ecsTaskExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy
```

#### ECS Task Role (Optional)
```bash
aws iam create-role \
  --role-name ecsTaskRole \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {"Service": "ecs-tasks.amazonaws.com"},
      "Action": "sts:AssumeRole"
    }]
  }'
```

---

## Project Architecture

### Application Structure
```
OrbitAOS.V6/
├── Controllers/          # MVC Controllers
├── Data/                 # Entity Framework DbContext and Migrations
├── Models/               # View Models
├── Views/                # Razor Views
├── wwwroot/              # Static files
├── appsettings.json      # Application configuration
├── Program.cs            # Application entry point
└── OrbitAOS.V6.csproj    # Project file
```

### Containerization Structure
```
OrbitAOS.V6/
├── Dockerfile                    # Multi-stage Docker build
├── .dockerignore                 # Docker ignore patterns
├── docker-compose.yml            # Local development compose
├── scripts/
│   ├── build-push.sh            # Build and push (Linux/Mac)
│   ├── build-push.bat           # Build and push (Windows)
│   ├── deploy-image.sh          # ECS deployment (Linux/Mac)
│   └── deploy-image.bat         # ECS deployment (Windows)
├── ecs/
│   ├── task-definition.json     # ECS task definition
│   └── service-definition.json  # ECS service definition
└── docs/
    └── DEPLOYMENT.md            # This file
```

---

## Local Development

### Running Locally with .NET CLI

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Update database connection string** in `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=OrbitAOS;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
     }
   }
   ```

3. **Apply database migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the application**:
   - URL: http://localhost:5000
   - Health check: http://localhost:5000/health

### Running Locally with Docker Compose

1. **Build and start the container**:
   ```bash
   docker-compose up --build
   ```

2. **Access the application**:
   - URL: http://localhost:8080
   - Health check: http://localhost:8080/health

3. **Stop the container**:
   ```bash
   docker-compose down
   ```

---

## Docker Containerization

### Dockerfile Overview

The Dockerfile uses a multi-stage build approach optimized for .NET applications:

1. **Builder Stage**: Uses `mcr.microsoft.com/dotnet/sdk:6.0`
   - Restores NuGet packages
   - Builds the application in Release mode
   - Publishes the application

2. **Runtime Stage**: Uses `mcr.microsoft.com/dotnet/sdk:6.0` (explicit base image)
   - Copies published artifacts
   - Creates non-root user for security
   - Sets environment variables
   - Exposes port 80

### Building the Docker Image Manually

```bash
# Build the image
docker build -t orbitaos-v6:latest .

# Run the container
docker run -d -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=OrbitAOS;User Id=sa;Password=YourPassword;TrustServerCertificate=True" \
  --name orbitaos-v6 \
  orbitaos-v6:latest

# View logs
docker logs -f orbitaos-v6

# Stop and remove
docker stop orbitaos-v6
docker rm orbitaos-v6
```

### Using Build Scripts

#### Linux/macOS
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

#### Windows
```cmd
scripts\build-push.bat
```

The script will:
1. Prompt for registry type (AWS ECR or Docker Hub)
2. Collect registry credentials
3. Authenticate with the registry
4. Build the Docker image
5. Push the image to the registry

---

## AWS ECS Fargate Deployment

### Architecture Overview

```
Internet
    ↓
Application Load Balancer (ALB)
    ↓
Target Group (IP target type)
    ↓
ECS Service (Fargate)
    ↓
ECS Tasks (2 replicas)
    ↓
Container (orbitaos-v6)
    ↓
CloudWatch Logs
```

### ECS Fargate Prerequisites

1. **VPC Configuration**:
   - VPC with at least 2 subnets in different AZs
   - Internet Gateway attached to VPC
   - Route table with route to Internet Gateway

2. **Security Group**:
   - Inbound: Port 80 (HTTP) from ALB or 0.0.0.0/0
   - Outbound: All traffic

3. **Database**:
   - SQL Server RDS instance or external database
   - Security group allowing connections from ECS tasks

### ECS Task Definition

The task definition (`ecs/task-definition.json`) specifies:

- **Launch Type**: FARGATE
- **Network Mode**: awsvpc (required for Fargate)
- **CPU**: 512 (.5 vCPU)
- **Memory**: 1024 MB (1 GB)
- **Container**:
  - Image: Your Docker image URI
  - Port: 80
  - Environment variables
  - Health check endpoint: `/health`
  - CloudWatch logging

**Valid Fargate CPU/Memory Combinations**:
- CPU: 256 → Memory: 512, 1024, 2048 MB
- CPU: 512 → Memory: 1024, 2048, 3072, 4096 MB
- CPU: 1024 → Memory: 2048-8192 MB (1 GB increments)
- CPU: 2048 → Memory: 4096-16384 MB (1 GB increments)
- CPU: 4096 → Memory: 8192-30720 MB (1 GB increments)

### ECS Service Definition

The service definition (`ecs/service-definition.json`) specifies:

- **Service Name**: orbitaos-v6-service
- **Desired Count**: 2 (for high availability)
- **Launch Type**: FARGATE
- **Network Configuration**: awsvpc with subnets and security groups
- **Load Balancer**: Optional ALB integration
- **Deployment Configuration**:
  - Maximum: 200% (allows rolling updates)
  - Minimum: 50% (maintains availability)
  - Circuit breaker enabled with rollback

### Deployment Steps

#### Step 1: Build and Push Docker Image

```bash
# Linux/macOS
./scripts/build-push.sh

# Windows
scripts\build-push.bat
```

Follow the prompts to:
1. Select registry (AWS ECR recommended)
2. Enter AWS region and account ID
3. Enter repository name (default: orbitaos-v6)
4. Enter image tag (default: latest)

**Example Output**:
```
Image: 123456789012.dkr.ecr.us-east-1.amazonaws.com/orbitaos-v6:latest
```

#### Step 2: Deploy to ECS Fargate

```bash
# Linux/macOS
./scripts/deploy-image.sh

# Windows
scripts\deploy-image.bat
```

The script will prompt for:

1. **AWS Region**: e.g., `us-east-1`
2. **ECS Cluster Name**: e.g., `my-ecs-cluster`
3. **VPC ID**: e.g., `vpc-0abc123def456`
4. **Subnet IDs**: e.g., `subnet-0abc123,subnet-0def456`
5. **Security Group ID**: e.g., `sg-0abc123def`
6. **Docker Image URI**: From Step 1
7. **Database Connection String**: Your SQL Server connection string
8. **Load Balancer**: y/n (recommended: yes)

**Example Connection String**:
```
Server=mydb.abc123.us-east-1.rds.amazonaws.com,1433;Database=OrbitAOS;User Id=admin;Password=MyPassword;TrustServerCertificate=True
```

#### Step 3: Verify Deployment

The script will:
1. Create/update ECS cluster
2. Create CloudWatch log group
3. Register task definition
4. Create/update ECS service
5. Create ALB and target group (if requested)
6. Wait for service to stabilize
7. Display deployment summary

**Deployment Summary Example**:
```
==========================================
Deployment Summary
==========================================
Cluster: my-ecs-cluster
Service: orbitaos-v6-service
Task Definition: arn:aws:ecs:us-east-1:123456789012:task-definition/orbitaos-v6-task:1
Region: us-east-1
Load Balancer DNS: http://orbitaos-v6-alb-123456789.us-east-1.elb.amazonaws.com
Health Check Endpoint: http://orbitaos-v6-alb-123456789.us-east-1.elb.amazonaws.com/health
CloudWatch Logs: /ecs/orbitaos-v6
```

#### Step 4: Access the Application

1. **Via Load Balancer** (if configured):
   ```
   http://orbitaos-v6-alb-123456789.us-east-1.elb.amazonaws.com
   ```

2. **Health Check**:
   ```
   http://orbitaos-v6-alb-123456789.us-east-1.elb.amazonaws.com/health
   ```

3. **View Logs**:
   ```bash
   aws logs tail /ecs/orbitaos-v6 --follow --region us-east-1
   ```

---

## Configuration Management

### Environment Variables

The application uses the following environment variables:

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `ASPNETCORE_ENVIRONMENT` | Application environment | Production | Yes |
| `ASPNETCORE_URLS` | Kestrel binding URLs | http://+:80 | Yes |
| `ConnectionStrings__DefaultConnection` | Database connection string | - | Yes |
| `ExternalUrls__AspNetCoreDocsUrl` | External documentation URL | https://docs.microsoft.com/aspnet/core | No |

### Configuration Files

1. **appsettings.json**: Base configuration
2. **appsettings.Production.json**: Production overrides
3. **appsettings.Development.json**: Development overrides

### Updating Configuration

#### Update Environment Variables in ECS

1. **Edit task definition**:
   ```bash
   # Download current task definition
   aws ecs describe-task-definition \
     --task-definition orbitaos-v6-task \
     --query 'taskDefinition' > task-def.json
   
   # Edit environment variables in task-def.json
   # Remove fields: taskDefinitionArn, revision, status, requiresAttributes, compatibilities, registeredAt, registeredBy
   
   # Register new revision
   aws ecs register-task-definition --cli-input-json file://task-def.json
   ```

2. **Update service**:
   ```bash
   aws ecs update-service \
     --cluster my-ecs-cluster \
     --service orbitaos-v6-service \
     --force-new-deployment
   ```

#### Use AWS Secrets Manager (Recommended)

1. **Store secrets**:
   ```bash
   aws secretsmanager create-secret \
     --name orbitaos-v6/db-connection \
     --secret-string "Server=mydb.abc123.us-east-1.rds.amazonaws.com;Database=OrbitAOS;User Id=admin;Password=MyPassword"
   ```

2. **Update task definition** to use secrets:
   ```json
   {
     "secrets": [
       {
         "name": "ConnectionStrings__DefaultConnection",
         "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:orbitaos-v6/db-connection"
       }
     ]
   }
   ```

3. **Grant permissions** to task execution role:
   ```bash
   aws iam attach-role-policy \
     --role-name ecsTaskExecutionRole \
     --policy-arn arn:aws:iam::aws:policy/SecretsManagerReadWrite
   ```

---

## Monitoring and Logging

### CloudWatch Logs

All container logs are sent to CloudWatch Logs:

- **Log Group**: `/ecs/orbitaos-v6`
- **Log Stream Prefix**: `ecs`

#### View Logs

```bash
# Tail logs in real-time
aws logs tail /ecs/orbitaos-v6 --follow --region us-east-1

# View logs for specific time range
aws logs filter-log-events \
  --log-group-name /ecs/orbitaos-v6 \
  --start-time $(date -d '1 hour ago' +%s)000 \
  --region us-east-1
```

#### CloudWatch Insights Queries

```sql
-- Error logs
fields @timestamp, @message
| filter @message like /ERROR/
| sort @timestamp desc
| limit 100

-- Slow requests
fields @timestamp, @message
| filter @message like /Request took/
| parse @message /Request took (?<duration>\d+)ms/
| filter duration > 1000
| sort duration desc
```

### Health Checks

The application exposes a health check endpoint at `/health`:

- **Endpoint**: `http://localhost/health`
- **Expected Response**: `200 OK` with "Healthy" status
- **Checks**: Application startup and readiness

#### ECS Service Health Checks

The ECS service uses the ALB target group health check:

- **Path**: `/health`
- **Interval**: 30 seconds
- **Timeout**: 5 seconds
- **Healthy Threshold**: 2
- **Unhealthy Threshold**: 3

### Application Insights (Optional)

To add Application Insights monitoring:

1. **Install NuGet package**:
   ```bash
   dotnet add package Microsoft.ApplicationInsights.AspNetCore
   ```

2. **Configure in Program.cs**:
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

3. **Add connection string** to environment variables:
   ```
   APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=...
   ```

---

## Troubleshooting

### Common Issues

#### 1. Task Fails to Start

**Symptoms**: Tasks start and immediately stop

**Possible Causes**:
- Invalid CPU/memory combination
- Image pull errors
- Application crashes on startup

**Solutions**:
```bash
# Check task stopped reason
aws ecs describe-tasks \
  --cluster my-ecs-cluster \
  --tasks <task-id> \
  --query 'tasks[0].stoppedReason'

# View container logs
aws logs tail /ecs/orbitaos-v6 --follow

# Check task definition
aws ecs describe-task-definition \
  --task-definition orbitaos-v6-task
```

#### 2. Database Connection Errors

**Symptoms**: Application starts but cannot connect to database

**Possible Causes**:
- Incorrect connection string
- Security group not allowing connections
- Database not accessible from ECS tasks

**Solutions**:
```bash
# Test database connectivity from ECS task
aws ecs execute-command \
  --cluster my-ecs-cluster \
  --task <task-id> \
  --container orbitaos-v6 \
  --interactive \
  --command "/bin/bash"

# Inside container, test connection
# (Note: SQL tools not included in runtime image)

# Check security groups
aws ec2 describe-security-groups \
  --group-ids <security-group-id>
```

#### 3. Load Balancer Health Check Failures

**Symptoms**: Tasks are running but marked unhealthy

**Possible Causes**:
- Health check path incorrect
- Application not responding on port 80
- Security group blocking ALB traffic

**Solutions**:
```bash
# Check target health
aws elbv2 describe-target-health \
  --target-group-arn <target-group-arn>

# View ALB access logs
# (Enable ALB access logs first)

# Test health endpoint directly
curl http://<task-private-ip>/health
```

#### 4. Service Not Scaling

**Symptoms**: Service stuck at 0 running tasks

**Possible Causes**:
- Insufficient capacity in subnets
- Service quota limits reached
- Task definition errors

**Solutions**:
```bash
# Check service events
aws ecs describe-services \
  --cluster my-ecs-cluster \
  --services orbitaos-v6-service \
  --query 'services[0].events'

# Check ECS capacity providers
aws ecs describe-capacity-providers

# Verify subnet availability
aws ec2 describe-subnets --subnet-ids <subnet-id>
```

### Debugging Commands

```bash
# List all tasks in service
aws ecs list-tasks \
  --cluster my-ecs-cluster \
  --service-name orbitaos-v6-service

# Describe specific task
aws ecs describe-tasks \
  --cluster my-ecs-cluster \
  --tasks <task-id>

# View service details
aws ecs describe-services \
  --cluster my-ecs-cluster \
  --services orbitaos-v6-service

# Check task definition
aws ecs describe-task-definition \
  --task-definition orbitaos-v6-task

# View CloudWatch logs
aws logs tail /ecs/orbitaos-v6 --follow

# Check ALB target health
aws elbv2 describe-target-health \
  --target-group-arn <target-group-arn>
```

---

## Security Considerations

### Container Security

1. **Non-Root User**: The Dockerfile creates and uses a non-root user (`appuser`)
2. **Minimal Base Image**: Uses official Microsoft runtime images
3. **No Unnecessary Tools**: Runtime image doesn't include curl, wget, or other tools
4. **Read-Only Filesystem**: Consider mounting volumes as read-only where possible

### Network Security

1. **Security Groups**:
   - Restrict inbound traffic to necessary ports only
   - Use separate security groups for ALB and ECS tasks
   - Allow ECS tasks to access database only

2. **VPC Configuration**:
   - Use private subnets for ECS tasks (with NAT Gateway)
   - Use public subnets for ALB only
   - Enable VPC Flow Logs for network monitoring

### Secrets Management

1. **Never hardcode secrets** in Dockerfile or task definition
2. **Use AWS Secrets Manager** for sensitive data
3. **Rotate secrets regularly**
4. **Use IAM roles** for AWS service access

### Application Security

1. **HTTPS**: Configure HTTPS in production
   ```csharp
   app.UseHttpsRedirection();
   app.UseHsts();
   ```

2. **Security Headers**: Add security headers middleware
   ```csharp
   app.Use(async (context, next) =>
   {
       context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
       context.Response.Headers.Add("X-Frame-Options", "DENY");
       context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
       await next();
   });
   ```

3. **CORS**: Configure CORS policies appropriately
4. **Authentication**: Use ASP.NET Core Identity with strong password policies

---

## Scaling and Performance

### Auto Scaling

#### Configure Service Auto Scaling

1. **Register scalable target**:
   ```bash
   aws application-autoscaling register-scalable-target \
     --service-namespace ecs \
     --resource-id service/my-ecs-cluster/orbitaos-v6-service \
     --scalable-dimension ecs:service:DesiredCount \
     --min-capacity 2 \
     --max-capacity 10
   ```

2. **Create scaling policy** (CPU-based):
   ```bash
   aws application-autoscaling put-scaling-policy \
     --service-namespace ecs \
     --resource-id service/my-ecs-cluster/orbitaos-v6-service \
     --scalable-dimension ecs:service:DesiredCount \
     --policy-name cpu-scaling-policy \
     --policy-type TargetTrackingScaling \
     --target-tracking-scaling-policy-configuration '{
       "TargetValue": 70.0,
       "PredefinedMetricSpecification": {
         "PredefinedMetricType": "ECSServiceAverageCPUUtilization"
       },
       "ScaleInCooldown": 300,
       "ScaleOutCooldown": 60
     }'
   ```

3. **Create scaling policy** (Request-based):
   ```bash
   aws application-autoscaling put-scaling-policy \
     --service-namespace ecs \
     --resource-id service/my-ecs-cluster/orbitaos-v6-service \
     --scalable-dimension ecs:service:DesiredCount \
     --policy-name request-scaling-policy \
     --policy-type TargetTrackingScaling \
     --target-tracking-scaling-policy-configuration '{
       "TargetValue": 1000.0,
       "PredefinedMetricSpecification": {
         "PredefinedMetricType": "ALBRequestCountPerTarget",
         "ResourceLabel": "app/orbitaos-v6-alb/1234567890/targetgroup/orbitaos-v6-tg/1234567890"
       }
     }'
   ```

### Performance Optimization

#### .NET Runtime Optimizations

1. **ReadyToRun Images**: Use ReadyToRun compilation for faster startup
   ```xml
   <PropertyGroup>
     <PublishReadyToRun>true</PublishReadyToRun>
   </PropertyGroup>
   ```

2. **Tiered Compilation**: Enable tiered compilation (default in .NET 6)
   ```
   DOTNET_TieredCompilation=1
   ```

3. **Server GC**: Use server garbage collection for better throughput
   ```xml
   <PropertyGroup>
     <ServerGarbageCollection>true</ServerGarbageCollection>
   </PropertyGroup>
   ```

#### Database Optimizations

1. **Connection Pooling**: Ensure connection pooling is enabled
   ```
   Server=...;Pooling=true;Min Pool Size=5;Max Pool Size=100
   ```

2. **Async Operations**: Use async/await for all database operations
3. **Indexes**: Add appropriate indexes to frequently queried columns
4. **Query Optimization**: Use EF Core query optimization techniques

#### Caching

1. **Response Caching**:
   ```csharp
   builder.Services.AddResponseCaching();
   app.UseResponseCaching();
   ```

2. **Distributed Cache** (Redis):
   ```csharp
   builder.Services.AddStackExchangeRedisCache(options =>
   {
       options.Configuration = "redis-endpoint:6379";
   });
   ```

### Blue/Green Deployments

1. **Create new task definition revision**
2. **Update service** with new task definition
3. **ECS automatically performs rolling update**:
   - Starts new tasks with new revision
   - Waits for health checks to pass
   - Drains connections from old tasks
   - Stops old tasks

4. **Rollback if needed**:
   ```bash
   aws ecs update-service \
     --cluster my-ecs-cluster \
     --service orbitaos-v6-service \
     --task-definition orbitaos-v6-task:1
   ```

---

## Additional Resources

### AWS Documentation
- [ECS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/AWS_Fargate.html)
- [ECS Task Definitions](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/task_definitions.html)
- [ECS Service Auto Scaling](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/service-auto-scaling.html)

### .NET Documentation
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)

### Best Practices
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [AWS ECS Best Practices](https://docs.aws.amazon.com/AmazonECS/latest/bestpracticesguide/intro.html)
- [.NET Performance Best Practices](https://docs.microsoft.com/dotnet/core/performance/)

---

## Support and Maintenance

### Regular Maintenance Tasks

1. **Update base images** regularly for security patches
2. **Monitor CloudWatch metrics** for performance issues
3. **Review and rotate secrets** periodically
4. **Update .NET runtime** to latest LTS version
5. **Review and optimize database queries**
6. **Monitor costs** and optimize resource allocation

### Monitoring Checklist

- [ ] CloudWatch alarms configured for CPU/Memory
- [ ] Log aggregation and analysis set up
- [ ] Health check endpoints monitored
- [ ] Database performance monitored
- [ ] Application Insights configured (optional)
- [ ] Cost monitoring enabled

---

## Conclusion

This deployment guide provides comprehensive instructions for containerizing and deploying the OrbitAOS.V6 ASP.NET Core application to AWS ECS Fargate. Follow the steps carefully, and refer to the troubleshooting section if you encounter any issues.

For questions or issues, consult the AWS and .NET documentation linked above, or contact your DevOps team.

**Happy Deploying! 🚀**
