# Quick Start Guide - API Gateway SAP Service Layer

## Inicio Rápido en 5 Minutos

### 1. Configurar Variables de Entorno

Crea un archivo `.env` en la raíz del proyecto:

```bash
# JWT Configuration
JWT_KEY=tu-clave-super-secreta-jwt-minimo-32-caracteres
JWT_ISSUER=MyApiGateway
JWT_AUDIENCE=MyApiClients

# SAP Service Layer
SERVICE_LAYER_BASE_URL=https://B1SERVER:50000/b1s/v1/
SERVICE_LAYER_COMPANY_DB=SBODemoGT
SERVICE_LAYER_USERNAME=manager
SERVICE_LAYER_PASSWORD=tu-password

# Redis
REDIS_CONFIGURATION=localhost:6379

# CORS (opcional para desarrollo)
CORS_ALLOWED_ORIGINS=http://localhost:3000
```

### 2. Iniciar Redis (Docker)

```bash
docker run -d --name redis -p 6379:6379 redis:latest
```

### 3. Ejecutar la Aplicación

```bash
cd improved-api
dotnet restore
dotnet run
```

### 4. Probar la API

Abre tu navegador en: `https://localhost:7194`

Verás la interfaz de Swagger UI donde puedes probar todos los endpoints.

### 5. Verificar Health Check

```bash
curl https://localhost:7194/health
```

Respuesta esperada:
```json
{
  "status": "Healthy",
  "results": {
    "redis": { "status": "Healthy" },
    "service-layer": { "status": "Healthy" }
  }
}
```

## Endpoints Principales

### Login (Establecer Sesión)
```bash
POST /api/v1/ServiceLayerGateway/login
Authorization: Bearer {tu-jwt-token}
```

### Proxy GET
```bash
GET /api/v1/ServiceLayerGateway/BusinessPartners
Authorization: Bearer {tu-jwt-token}
```

### Crear Cotización
```bash
POST /api/v1/ServiceLayerGateway/quotations
Authorization: Bearer {tu-jwt-token}
Content-Type: application/json

{
  "cardCode": "C00001",
  "docDate": "2026-01-05T00:00:00",
  "documentLines": [
    {
      "itemCode": "A00001",
      "quantity": 10,
      "price": 100.50
    }
  ]
}
```

## Solución de Problemas Comunes

### Error: "JWT Key is not configured"
**Solución**: Asegúrate de que la variable `JWT_KEY` esté configurada en tu `.env` o User Secrets.

### Error: "Failed to connect to Redis"
**Solución**: Verifica que Redis esté ejecutándose:
```bash
docker ps | grep redis
```

### Error: "Service Layer Base URL is not configured"
**Solución**: Configura `SERVICE_LAYER_BASE_URL` en tu `.env`.

### Error 429 (Too Many Requests)
**Solución**: Has excedido el límite de tasa. Espera 60 segundos o ajusta la configuración de rate limiting.

## Documentación Completa

- **README.md** - Documentación completa del proyecto
- **SECURITY.md** - Guías de seguridad
- **DEPLOYMENT.md** - Guía de despliegue
- **MEJORAS_REALIZADAS.md** - Resumen de mejoras (español)

## Soporte

¿Necesitas ayuda? Consulta la documentación completa o contacta: support@tudominio.com
