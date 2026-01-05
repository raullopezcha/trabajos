# Deployment Guide

## Prerequisites

Before deploying the SAP Service Layer API Gateway, ensure you have:

- .NET 8.0 SDK installed
- Redis server (local, Azure Redis Cache, or AWS ElastiCache)
- SAP Business One with Service Layer enabled
- Valid SSL/TLS certificate for production
- Access to environment variable configuration

## Local Development Deployment

### 1. Clone and Setup

```bash
# Navigate to project directory
cd improved-api

# Restore dependencies
dotnet restore

# Setup User Secrets
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-super-secret-jwt-key-min-32-characters-long"
dotnet user-secrets set "ServiceLayer:CompanyDB" "SBODemoGT"
dotnet user-secrets set "ServiceLayer:UserName" "manager"
dotnet user-secrets set "ServiceLayer:Password" "your-password"
dotnet user-secrets set "Redis:Configuration" "localhost:6379"
```

### 2. Run Redis (Docker)

```bash
docker run -d --name redis -p 6379:6379 redis:latest
```

### 3. Build and Run

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Or run with specific environment
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### 4. Verify Deployment

```bash
# Check health endpoint
curl https://localhost:7194/health

# Access Swagger UI
open https://localhost:7194
```

## Docker Deployment

### 1. Create Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiGateServiceLayer.csproj", "./"]
RUN dotnet restore "ApiGateServiceLayer.csproj"
COPY . .
RUN dotnet build "ApiGateServiceLayer.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ApiGateServiceLayer.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateServiceLayer.dll"]
```

### 2. Create docker-compose.yml

```yaml
version: '3.8'

services:
  api-gateway:
    build: .
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - JWT_KEY=${JWT_KEY}
      - JWT_ISSUER=MyApiGateway
      - JWT_AUDIENCE=MyApiClients
      - SERVICE_LAYER_BASE_URL=${SERVICE_LAYER_BASE_URL}
      - SERVICE_LAYER_COMPANY_DB=${SERVICE_LAYER_COMPANY_DB}
      - SERVICE_LAYER_USERNAME=${SERVICE_LAYER_USERNAME}
      - SERVICE_LAYER_PASSWORD=${SERVICE_LAYER_PASSWORD}
      - REDIS_CONFIGURATION=redis:6379
      - CORS_ALLOWED_ORIGINS=${CORS_ALLOWED_ORIGINS}
    depends_on:
      - redis
    networks:
      - api-network

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    networks:
      - api-network

volumes:
  redis-data:

networks:
  api-network:
    driver: bridge
```

### 3. Build and Run

```bash
# Create .env file with secrets
cat > .env << EOF
JWT_KEY=your-super-secret-jwt-key-min-32-characters-long
SERVICE_LAYER_BASE_URL=https://B1SERVER:50000/b1s/v1/
SERVICE_LAYER_COMPANY_DB=SBODemoGT
SERVICE_LAYER_USERNAME=manager
SERVICE_LAYER_PASSWORD=your-password
CORS_ALLOWED_ORIGINS=https://yourdomain.com
EOF

# Build and run
docker-compose up -d

# View logs
docker-compose logs -f api-gateway

# Check health
curl http://localhost:8080/health
```

## Azure App Service Deployment

### 1. Create Azure Resources

```bash
# Login to Azure
az login

# Create resource group
az group create --name rg-api-gateway --location eastus

# Create App Service Plan
az appservice plan create \
  --name plan-api-gateway \
  --resource-group rg-api-gateway \
  --sku B1 \
  --is-linux

# Create Web App
az webapp create \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --plan plan-api-gateway \
  --runtime "DOTNET|8.0"

# Create Azure Redis Cache
az redis create \
  --name redis-api-gateway \
  --resource-group rg-api-gateway \
  --location eastus \
  --sku Basic \
  --vm-size c0
```

### 2. Configure Application Settings

```bash
# Set environment variables
az webapp config appsettings set \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    JWT_KEY="your-super-secret-jwt-key-min-32-characters-long" \
    JWT_ISSUER=MyApiGateway \
    JWT_AUDIENCE=MyApiClients \
    SERVICE_LAYER_BASE_URL="https://B1SERVER:50000/b1s/v1/" \
    SERVICE_LAYER_COMPANY_DB=SBODemoGT \
    SERVICE_LAYER_USERNAME=manager \
    SERVICE_LAYER_PASSWORD="your-password" \
    REDIS_CONFIGURATION="redis-api-gateway.redis.cache.windows.net:6380,ssl=True,password=<redis-key>" \
    CORS_ALLOWED_ORIGINS="https://yourdomain.com"
```

### 3. Deploy Application

```bash
# Publish locally
dotnet publish -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to Azure
az webapp deployment source config-zip \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --src deploy.zip

# Enable HTTPS only
az webapp update \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --https-only true
```

### 4. Configure Custom Domain and SSL

```bash
# Add custom domain
az webapp config hostname add \
  --webapp-name api-gateway-sap \
  --resource-group rg-api-gateway \
  --hostname api.yourdomain.com

# Bind SSL certificate
az webapp config ssl bind \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --certificate-thumbprint <thumbprint> \
  --ssl-type SNI
```

## AWS Elastic Beanstalk Deployment

### 1. Install EB CLI

```bash
pip install awsebcli
```

### 2. Initialize EB Application

```bash
# Initialize
eb init -p "64bit Amazon Linux 2023 v3.0.0 running .NET 8" api-gateway-sap --region us-east-1

# Create environment
eb create api-gateway-prod \
  --instance-type t3.small \
  --envvars \
    ASPNETCORE_ENVIRONMENT=Production,\
    JWT_KEY=your-super-secret-jwt-key-min-32-characters-long,\
    JWT_ISSUER=MyApiGateway,\
    JWT_AUDIENCE=MyApiClients,\
    SERVICE_LAYER_BASE_URL=https://B1SERVER:50000/b1s/v1/,\
    SERVICE_LAYER_COMPANY_DB=SBODemoGT,\
    SERVICE_LAYER_USERNAME=manager,\
    SERVICE_LAYER_PASSWORD=your-password,\
    REDIS_CONFIGURATION=your-elasticache-endpoint:6379,\
    CORS_ALLOWED_ORIGINS=https://yourdomain.com
```

### 3. Deploy

```bash
# Deploy application
eb deploy

# Open in browser
eb open

# View logs
eb logs
```

## Kubernetes Deployment

### 1. Create Kubernetes Manifests

**namespace.yaml**
```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: api-gateway
```

**secrets.yaml**
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: api-gateway-secrets
  namespace: api-gateway
type: Opaque
stringData:
  jwt-key: "your-super-secret-jwt-key-min-32-characters-long"
  service-layer-username: "manager"
  service-layer-password: "your-password"
  redis-connection: "redis:6379"
```

**configmap.yaml**
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: api-gateway-config
  namespace: api-gateway
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  JWT_ISSUER: "MyApiGateway"
  JWT_AUDIENCE: "MyApiClients"
  SERVICE_LAYER_BASE_URL: "https://B1SERVER:50000/b1s/v1/"
  SERVICE_LAYER_COMPANY_DB: "SBODemoGT"
  CORS_ALLOWED_ORIGINS: "https://yourdomain.com"
```

**deployment.yaml**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-gateway
  namespace: api-gateway
spec:
  replicas: 3
  selector:
    matchLabels:
      app: api-gateway
  template:
    metadata:
      labels:
        app: api-gateway
    spec:
      containers:
      - name: api-gateway
        image: your-registry/api-gateway:latest
        ports:
        - containerPort: 80
        env:
        - name: JWT_KEY
          valueFrom:
            secretKeyRef:
              name: api-gateway-secrets
              key: jwt-key
        - name: SERVICE_LAYER_USERNAME
          valueFrom:
            secretKeyRef:
              name: api-gateway-secrets
              key: service-layer-username
        - name: SERVICE_LAYER_PASSWORD
          valueFrom:
            secretKeyRef:
              name: api-gateway-secrets
              key: service-layer-password
        - name: REDIS_CONFIGURATION
          valueFrom:
            secretKeyRef:
              name: api-gateway-secrets
              key: redis-connection
        envFrom:
        - configMapRef:
            name: api-gateway-config
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 5
```

**service.yaml**
```yaml
apiVersion: v1
kind: Service
metadata:
  name: api-gateway-service
  namespace: api-gateway
spec:
  type: LoadBalancer
  ports:
  - port: 80
    targetPort: 80
    protocol: TCP
  selector:
    app: api-gateway
```

### 2. Deploy to Kubernetes

```bash
# Apply manifests
kubectl apply -f namespace.yaml
kubectl apply -f secrets.yaml
kubectl apply -f configmap.yaml
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml

# Check status
kubectl get pods -n api-gateway
kubectl get svc -n api-gateway

# View logs
kubectl logs -f deployment/api-gateway -n api-gateway
```

## Post-Deployment Verification

### 1. Health Check

```bash
curl https://your-domain.com/health
```

Expected response:
```json
{
  "status": "Healthy",
  "results": {
    "redis": { "status": "Healthy" },
    "service-layer": { "status": "Healthy" }
  }
}
```

### 2. Swagger UI

Navigate to: `https://your-domain.com`

### 3. Test Authentication

```bash
# Test login endpoint
curl -X POST https://your-domain.com/api/v1/ServiceLayerGateway/login \
  -H "Authorization: Bearer {your-jwt-token}"
```

### 4. Monitor Logs

Check application logs for errors:
- Azure: Application Insights or Log Stream
- AWS: CloudWatch Logs
- Kubernetes: `kubectl logs`
- Docker: `docker logs`

## Monitoring and Maintenance

### Application Insights (Azure)

```bash
# Enable Application Insights
az monitor app-insights component create \
  --app api-gateway-insights \
  --location eastus \
  --resource-group rg-api-gateway

# Get instrumentation key
az monitor app-insights component show \
  --app api-gateway-insights \
  --resource-group rg-api-gateway \
  --query instrumentationKey

# Add to app settings
az webapp config appsettings set \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --settings APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=<key>"
```

### CloudWatch (AWS)

Logs are automatically sent to CloudWatch Logs when deployed to Elastic Beanstalk.

### Prometheus & Grafana (Kubernetes)

Install Prometheus and Grafana for monitoring:

```bash
# Add Prometheus Helm repo
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update

# Install Prometheus
helm install prometheus prometheus-community/kube-prometheus-stack -n monitoring --create-namespace
```

## Troubleshooting

### Common Issues

1. **Health check failing**
   - Verify Redis connection
   - Check Service Layer URL accessibility
   - Review application logs

2. **JWT authentication not working**
   - Verify JWT_KEY is set correctly
   - Check token expiration
   - Validate issuer and audience

3. **Rate limiting too aggressive**
   - Adjust RATE_LIMIT_PERMIT_LIMIT
   - Increase RATE_LIMIT_WINDOW_SECONDS

4. **CORS errors**
   - Add origin to CORS_ALLOWED_ORIGINS
   - Verify origin format (include protocol)

## Rollback Procedures

### Azure App Service

```bash
# List deployment slots
az webapp deployment slot list \
  --name api-gateway-sap \
  --resource-group rg-api-gateway

# Swap slots
az webapp deployment slot swap \
  --name api-gateway-sap \
  --resource-group rg-api-gateway \
  --slot staging \
  --target-slot production
```

### Kubernetes

```bash
# Rollback to previous version
kubectl rollout undo deployment/api-gateway -n api-gateway

# Check rollout status
kubectl rollout status deployment/api-gateway -n api-gateway
```

### Docker Compose

```bash
# Stop current version
docker-compose down

# Checkout previous version
git checkout <previous-commit>

# Rebuild and start
docker-compose up -d --build
```

## Security Hardening

1. **Enable firewall rules** - Restrict access to known IPs
2. **Use managed identities** - Avoid storing credentials
3. **Enable audit logging** - Track all access
4. **Regular updates** - Keep dependencies current
5. **Penetration testing** - Regular security assessments

## Support

For deployment issues:
- Check logs first
- Review health check endpoint
- Consult SECURITY.md for security issues
- Contact: devops@yourdomain.com
