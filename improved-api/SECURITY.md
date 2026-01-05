# Security Guidelines

## Overview

This document outlines the security measures implemented in the SAP Service Layer API Gateway and provides guidelines for secure deployment and operation.

## Security Features

### 1. Authentication & Authorization

#### JWT Token Authentication
- **Algorithm**: HMAC-SHA256
- **Key Length**: Minimum 32 characters (256 bits)
- **Token Lifetime**: Configurable (recommended: 1 hour)
- **Claims Validation**: Issuer, Audience, Lifetime, Signature

**Best Practices:**
- Use strong, randomly generated keys (min 32 characters)
- Rotate JWT keys periodically (every 90 days)
- Store keys in secure vaults (Azure Key Vault, AWS Secrets Manager)
- Never commit keys to source control

#### Authorization
- All proxy endpoints require valid JWT token
- Use `[Authorize]` attribute on sensitive endpoints
- Implement role-based access control (RBAC) if needed

### 2. Configuration Security

#### Sensitive Data Protection
All sensitive configuration moved to:
- **Environment Variables** (Production)
- **User Secrets** (Development)
- **Azure Key Vault / AWS Secrets Manager** (Enterprise)

**Never store in code:**
- JWT signing keys
- SAP credentials
- Redis connection strings
- API keys

#### Configuration Hierarchy
1. Environment Variables (highest priority)
2. User Secrets (development)
3. appsettings.json (non-sensitive only)

### 3. Network Security

#### HTTPS/TLS
- **Enforced in Production**: `UseHttpsRedirection()`
- **HSTS Enabled**: HTTP Strict Transport Security
- **Minimum TLS Version**: TLS 1.2
- **Certificate**: Use valid SSL/TLS certificates

#### CORS (Cross-Origin Resource Sharing)
- **Whitelist Origins**: Only allow trusted domains
- **Credentials**: Enabled for authenticated requests
- **Methods**: Restricted to necessary HTTP methods

**Configuration:**
```bash
CORS_ALLOWED_ORIGINS=https://app.yourdomain.com,https://admin.yourdomain.com
```

### 4. Rate Limiting

#### Protection Against Abuse
- **Default Limit**: 100 requests per 60 seconds
- **Per User/IP**: Partitioned by authenticated user or IP address
- **Queue Limit**: 10 requests
- **Response**: 429 Too Many Requests

**Customization:**
```bash
RATE_LIMIT_PERMIT_LIMIT=100
RATE_LIMIT_WINDOW_SECONDS=60
RATE_LIMIT_QUEUE_LIMIT=10
```

### 5. Input Validation

#### Data Validation
- **Data Annotations**: All DTOs include validation attributes
- **Model State Validation**: Automatic validation before controller actions
- **Error Messages**: User-friendly, non-revealing

**Validation Rules:**
- Required fields: `[Required]`
- String length: `[StringLength(max, MinimumLength = min)]`
- Numeric ranges: `[Range(min, max)]`
- Collection size: `[MinLength(n)]`

#### SQL Injection Prevention
- **OData Queries**: SAP Service Layer handles query sanitization
- **No Direct SQL**: All data access through Service Layer API

### 6. Error Handling

#### Global Exception Middleware
- **Consistent Responses**: Standardized error format
- **Information Disclosure**: Detailed errors only in development
- **Logging**: All exceptions logged with context

**Production Error Response:**
```json
{
  "success": false,
  "errorMessage": "An internal server error occurred.",
  "statusCode": 500,
  "timestamp": "2026-01-05T12:00:00Z"
}
```

**Development Error Response:**
```json
{
  "success": false,
  "errorMessage": "An internal server error occurred.",
  "errorDetails": "Detailed stack trace...",
  "statusCode": 500,
  "timestamp": "2026-01-05T12:00:00Z"
}
```

### 7. Logging & Monitoring

#### Structured Logging (Serilog)
- **No Sensitive Data**: Never log passwords, tokens, or PII
- **Request Logging**: HTTP method, path, status, duration
- **Error Logging**: Exception details with context
- **Log Rotation**: Daily log files

**Logged Information:**
- Request method and path
- Response status code
- Execution time
- User identity (username, not credentials)
- IP address
- Exception messages and stack traces

**Never Log:**
- Passwords
- JWT tokens
- API keys
- Credit card numbers
- Personal identifiable information (PII)

#### Health Checks
- **Redis**: Connection and availability
- **Service Layer**: Endpoint reachability
- **Endpoint**: `/health`

### 8. Session Management

#### Redis Session Storage
- **Encryption**: Redis connection with SSL/TLS
- **Expiration**: 30 minutes automatic expiration
- **Thread-Safe**: SemaphoreSlim for concurrent access
- **Isolation**: Separate keys per environment

**Security Measures:**
- Use Redis with authentication (`password=...`)
- Enable SSL/TLS (`ssl=True`)
- Set appropriate expiration times
- Monitor Redis access logs

### 9. Resilience & Fault Tolerance

#### Polly Policies

**Retry Policy:**
- 3 retries with exponential backoff (2s, 4s, 8s)
- Handles transient HTTP errors (5xx, network failures)
- Logs retry attempts

**Circuit Breaker:**
- Opens after 5 consecutive failures
- Stays open for 30 seconds
- Prevents cascading failures
- Logs circuit state changes

### 10. Dependency Security

#### NuGet Package Management
- **Regular Updates**: Keep packages up-to-date
- **Vulnerability Scanning**: Use `dotnet list package --vulnerable`
- **Trusted Sources**: Only use official NuGet.org packages

**Update Command:**
```bash
dotnet list package --outdated
dotnet add package <PackageName> --version <LatestVersion>
```

## Deployment Security

### 1. Environment Separation
- **Development**: Local development with User Secrets
- **Staging**: Pre-production with environment variables
- **Production**: Production with secure vaults

### 2. Container Security (Docker)
```dockerfile
# Use official Microsoft images
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Run as non-root user
USER app

# Copy only necessary files
COPY --chown=app:app ./publish /app

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
```

### 3. Kubernetes Security
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: api-gateway-secrets
type: Opaque
data:
  jwt-key: <base64-encoded-key>
  redis-connection: <base64-encoded-connection>
---
apiVersion: apps/v1
kind: Deployment
spec:
  template:
    spec:
      containers:
      - name: api-gateway
        env:
        - name: JWT_KEY
          valueFrom:
            secretKeyRef:
              name: api-gateway-secrets
              key: jwt-key
```

### 4. Azure App Service
- Use **Managed Identity** for Azure resources
- Store secrets in **Azure Key Vault**
- Enable **Application Insights** for monitoring
- Configure **IP Restrictions** if needed

### 5. AWS Elastic Beanstalk
- Use **IAM Roles** for AWS resources
- Store secrets in **AWS Secrets Manager**
- Enable **CloudWatch** for logging
- Configure **Security Groups** appropriately

## Security Checklist

### Pre-Deployment
- [ ] All sensitive configuration in environment variables
- [ ] JWT key is strong (min 32 characters)
- [ ] HTTPS enforced in production
- [ ] CORS configured with specific origins
- [ ] Rate limiting enabled and configured
- [ ] Input validation on all endpoints
- [ ] Global exception handling tested
- [ ] Logging configured (no sensitive data)
- [ ] Health checks working
- [ ] Dependencies updated and scanned

### Post-Deployment
- [ ] SSL/TLS certificate valid and not expired
- [ ] Health check endpoint accessible
- [ ] Logs being written correctly
- [ ] Rate limiting working as expected
- [ ] CORS policy tested from allowed origins
- [ ] JWT authentication working
- [ ] Redis connection secure (SSL enabled)
- [ ] SAP Service Layer connection working
- [ ] Error responses don't leak sensitive info
- [ ] Monitoring and alerting configured

## Incident Response

### Security Incident Procedure
1. **Detect**: Monitor logs and health checks
2. **Contain**: Disable affected endpoints if necessary
3. **Investigate**: Review logs and identify root cause
4. **Remediate**: Apply fixes and patches
5. **Document**: Record incident details and lessons learned

### Common Security Issues

#### 1. JWT Token Compromised
- Rotate JWT signing key immediately
- Invalidate all existing tokens
- Force users to re-authenticate
- Investigate how token was compromised

#### 2. Rate Limit Bypass
- Review rate limiting configuration
- Check for IP spoofing
- Implement additional throttling if needed

#### 3. Unauthorized Access
- Review authentication logs
- Check for brute force attempts
- Implement account lockout if needed
- Review CORS configuration

#### 4. Data Breach
- Identify affected data
- Notify affected parties
- Review access logs
- Implement additional security measures

## Compliance

### GDPR (General Data Protection Regulation)
- **Data Minimization**: Only collect necessary data
- **Right to Erasure**: Implement data deletion
- **Data Portability**: Provide data export
- **Consent**: Obtain explicit consent for data processing

### PCI DSS (Payment Card Industry Data Security Standard)
- **No Card Data Storage**: Never store credit card numbers
- **Encryption**: Use TLS for all communications
- **Access Control**: Implement strong authentication
- **Logging**: Maintain audit trails

### HIPAA (Health Insurance Portability and Accountability Act)
- **PHI Protection**: Encrypt protected health information
- **Access Logs**: Maintain detailed access logs
- **Audit Controls**: Implement audit mechanisms
- **Data Integrity**: Ensure data accuracy

## Security Contacts

For security issues or vulnerabilities:
- **Email**: security@yourdomain.com
- **Response Time**: 24 hours for critical issues
- **Disclosure**: Responsible disclosure policy

## References

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Redis Security](https://redis.io/topics/security)

## Version History

- **v1.0** (2026-01-05): Initial security documentation
