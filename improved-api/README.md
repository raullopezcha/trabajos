# SAP Service Layer API Gateway - Improved & Secured

A professional, secure API Gateway for SAP Business One Service Layer with advanced features including session management, rate limiting, resilience patterns, and comprehensive security measures.

## 🚀 Features

### Security Enhancements
- ✅ **JWT Authentication** - Secure token-based authentication with configurable validation
- ✅ **Rate Limiting** - Prevents API abuse with configurable limits per user/IP
- ✅ **CORS Policy** - Properly configured Cross-Origin Resource Sharing
- ✅ **Input Validation** - Comprehensive data validation with Data Annotations
- ✅ **Secure Configuration** - Sensitive data moved to environment variables and User Secrets
- ✅ **Global Exception Handling** - Consistent error responses with detailed logging

### Resilience & Performance
- ✅ **Polly Retry Policy** - Automatic retry with exponential backoff (3 retries: 2s, 4s, 8s)
- ✅ **Circuit Breaker** - Prevents cascading failures (opens after 5 failures, 30s break)
- ✅ **Session Management** - Shared SAP session with automatic re-authentication
- ✅ **Redis Caching** - Fast session storage with automatic expiration
- ✅ **Thread-Safe Operations** - SemaphoreSlim for concurrent login protection

### Observability
- ✅ **Structured Logging** - Serilog with console and file outputs
- ✅ **Request Logging** - Automatic HTTP request/response timing
- ✅ **Health Checks** - Monitor Redis and Service Layer connectivity
- ✅ **Swagger/OpenAPI** - Interactive API documentation

## 📋 Prerequisites

- .NET 8.0 SDK or later
- Redis server (local or Azure Redis Cache)
- SAP Business One with Service Layer enabled
- Valid SAP credentials

## 🔧 Configuration

### 1. Environment Variables (Recommended for Production)

Create a `.env` file or set environment variables:

```bash
# JWT Configuration
JWT_KEY=your-super-secret-jwt-key-min-32-characters-long
JWT_ISSUER=MyApiGateway
JWT_AUDIENCE=MyApiClients

# SAP Service Layer Configuration
SERVICE_LAYER_BASE_URL=https://B1SERVER:50000/b1s/v1/
SERVICE_LAYER_COMPANY_DB=SBODemoGT
SERVICE_LAYER_USERNAME=manager
SERVICE_LAYER_PASSWORD=your-password-here

# Redis Configuration
REDIS_CONFIGURATION=your-redis-connection-string-here

# CORS Configuration (comma-separated)
CORS_ALLOWED_ORIGINS=https://yourdomain.com,https://app.yourdomain.com

# Rate Limiting
RATE_LIMIT_PERMIT_LIMIT=100
RATE_LIMIT_WINDOW_SECONDS=60
RATE_LIMIT_QUEUE_LIMIT=10
```

### 2. User Secrets (Recommended for Development)

```bash
cd improved-api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-super-secret-jwt-key-min-32-characters-long"
dotnet user-secrets set "ServiceLayer:CompanyDB" "SBODemoGT"
dotnet user-secrets set "ServiceLayer:UserName" "manager"
dotnet user-secrets set "ServiceLayer:Password" "your-password"
dotnet user-secrets set "Redis:Configuration" "your-redis-connection-string"
```

### 3. Configuration Files

- `appsettings.json` - Base configuration (no secrets)
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides

## 🏃 Running the Application

### Development

```bash
cd improved-api
dotnet restore
dotnet build
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7194`
- HTTP: `http://localhost:5084`
- Swagger UI: `https://localhost:7194` (root)

### Production

```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet ApiGateServiceLayer.dll
```

## 📚 API Endpoints

### Authentication
- `POST /api/v1/ServiceLayerGateway/login` - Establish SAP session

### Proxy Endpoints (Require JWT)
- `GET /api/v1/ServiceLayerGateway/{**path}` - Proxy GET requests to Service Layer
- `POST /api/v1/ServiceLayerGateway/{**path}` - Proxy POST requests to Service Layer

### Specialized Endpoints (Require JWT)
- `POST /api/v1/ServiceLayerGateway/quotations` - Create sales quotation
- `POST /api/v1/ServiceLayerGateway/batch/create-quotation-and-get-partner` - Batch operation

### Health & Monitoring
- `GET /health` - Health check endpoint (Redis + Service Layer)

## 🔐 Security Best Practices

### 1. JWT Token Generation
Generate JWT tokens with proper claims:

```csharp
var claims = new[]
{
    new Claim(ClaimTypes.Name, username),
    new Claim(ClaimTypes.NameIdentifier, userId),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: jwtIssuer,
    audience: jwtAudience,
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: creds
);

return new JwtSecurityTokenHandler().WriteToken(token);
```

### 2. HTTPS Only in Production
Ensure `UseHttpsRedirection()` is enabled and configure HSTS:

```csharp
app.UseHsts(); // Already configured in production mode
```

### 3. Rate Limiting
Current configuration:
- 100 requests per 60 seconds per user/IP
- Queue limit: 10 requests
- Returns 429 (Too Many Requests) when exceeded

### 4. Input Validation
All DTOs include validation attributes:
- `[Required]` - Mandatory fields
- `[StringLength]` - Length constraints
- `[Range]` - Numeric ranges
- `[MinLength]` - Collection minimums

## 📊 Monitoring & Logging

### Log Files
Logs are written to:
- Console (structured JSON in production)
- File: `logs/api-gateway-YYYYMMDD.log` (daily rotation)

### Log Levels
- **Development**: Debug and above
- **Production**: Warning and above

### Health Checks
Monitor application health:

```bash
curl https://localhost:7194/health
```

Response:
```json
{
  "status": "Healthy",
  "results": {
    "redis": { "status": "Healthy" },
    "service-layer": { "status": "Healthy" }
  }
}
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to `https://localhost:7194`
2. Click "Authorize" and enter your JWT token: `Bearer {your-token}`
3. Test endpoints interactively

### Using cURL

```bash
# Login to establish session
curl -X POST https://localhost:7194/api/v1/ServiceLayerGateway/login \
  -H "Authorization: Bearer {your-jwt-token}"

# Get business partners
curl -X GET https://localhost:7194/api/v1/ServiceLayerGateway/BusinessPartners \
  -H "Authorization: Bearer {your-jwt-token}"

# Create quotation
curl -X POST https://localhost:7194/api/v1/ServiceLayerGateway/quotations \
  -H "Authorization: Bearer {your-jwt-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "cardCode": "C00001",
    "docDate": "2026-01-05T00:00:00",
    "documentLines": [
      {
        "itemCode": "A00001",
        "quantity": 10,
        "price": 100.50
      }
    ]
  }'
```

## 🏗️ Architecture

### Components

1. **Controllers** - API endpoints with validation and authorization
2. **Services** - Business logic and SAP Service Layer communication
3. **Middleware** - Request logging and global exception handling
4. **Models** - DTOs with validation attributes

### Design Patterns

- **Repository Pattern** - `IServiceLayerClient` abstraction
- **Factory Pattern** - `IHttpClientFactory` for HTTP clients
- **Retry Pattern** - Polly retry with exponential backoff
- **Circuit Breaker Pattern** - Polly circuit breaker for fault tolerance
- **Singleton Pattern** - Redis connection and session storage

## 🔄 Session Management

The gateway maintains a shared SAP session:

1. First request triggers login to SAP Service Layer
2. Session cookies (B1SESSION, ROUTEID) stored in Redis
3. Subsequent requests reuse the session
4. Automatic re-authentication on 401 Unauthorized
5. Thread-safe login with SemaphoreSlim
6. Session expires after 30 minutes of inactivity

## 🚨 Error Handling

All errors return consistent JSON responses:

```json
{
  "success": false,
  "errorMessage": "User-friendly error message",
  "errorDetails": "Detailed error (development only)",
  "statusCode": 500,
  "timestamp": "2026-01-05T12:00:00Z"
}
```

## 📦 Dependencies

- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **Microsoft.AspNetCore.RateLimiting** - Rate limiting
- **Microsoft.Extensions.Http.Polly** - Resilience policies
- **Serilog.AspNetCore** - Structured logging
- **StackExchange.Redis** - Redis client
- **AspNetCore.HealthChecks.Redis** - Redis health checks
- **AspNetCore.HealthChecks.Uris** - HTTP health checks
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI

## 🔒 Security Checklist

- [x] Sensitive configuration in environment variables
- [x] JWT authentication with strong keys (min 32 characters)
- [x] HTTPS enforced in production
- [x] CORS properly configured
- [x] Rate limiting enabled
- [x] Input validation on all endpoints
- [x] Global exception handling
- [x] Structured logging (no sensitive data logged)
- [x] Health checks for monitoring
- [x] Thread-safe session management
- [x] Circuit breaker for external dependencies
- [x] Retry policy with exponential backoff

## 📝 License

This project is provided as-is for educational and commercial use.

## 🤝 Support

For issues or questions:
- Check logs in `logs/` directory
- Review health check endpoint: `/health`
- Consult Swagger documentation at root URL
- Contact: support@yourdomain.com

## 🔄 Version History

### v1.0 - Improved & Secured (2026-01-05)
- ✅ Security enhancements
- ✅ Structured logging with Serilog
- ✅ Rate limiting
- ✅ CORS configuration
- ✅ Global exception handling
- ✅ Input validation
- ✅ Improved documentation
- ✅ Thread-safe session management
