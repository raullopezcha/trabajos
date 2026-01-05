using ApiGateServiceLayer.Middleware;
using ApiGateServiceLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using StackExchange.Redis;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

/*
  Proyecto: ApiGateServiceLayer - Improved & Secured
  Descripción: Web API en C# que actúa como API Gateway hacia SAP Business One Service Layer,
  reutilizando la misma sesión (B1SESSION/ROUTEID) para múltiples usuarios con manejo automático de expiración
  y políticas de resiliencia mediante Polly (Retry + Circuit Breaker).
  
  Mejoras de Seguridad Implementadas:
  - Configuración sensible movida a variables de entorno y User Secrets
  - Validación de entrada con Data Annotations
  - Logging estructurado con Serilog
  - Rate Limiting para prevenir abuso
  - CORS configurado correctamente
  - Manejo global de excepciones
  - Documentación mejorada con Swagger/OpenAPI
  - Health Checks para monitoreo
  - Thread-safe session management con SemaphoreSlim
*/

// 1. Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/api-gateway-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting API Gateway Service Layer application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Load configuration from environment variables
    builder.Configuration.AddEnvironmentVariables();

    // Configuración
    var jwtKey = builder.Configuration["JWT_KEY"] ?? builder.Configuration["Jwt:Key"];
    var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? builder.Configuration["Jwt:Audience"];
    var slBaseUrl = builder.Configuration["SERVICE_LAYER_BASE_URL"] ?? builder.Configuration["ServiceLayer:BaseUrl"];
    var redisConf = builder.Configuration["REDIS_CONFIGURATION"] ?? builder.Configuration["Redis:Configuration"];

    // Validate critical configuration
    if (string.IsNullOrEmpty(jwtKey))
        throw new InvalidOperationException("JWT Key is not configured. Set JWT_KEY environment variable or Jwt:Key in configuration.");
    if (string.IsNullOrEmpty(slBaseUrl))
        throw new InvalidOperationException("Service Layer Base URL is not configured.");
    if (string.IsNullOrEmpty(redisConf))
        throw new InvalidOperationException("Redis configuration is not configured.");

    // 2. JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("JWT Authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Debug("JWT Token validated for user: {User}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();

    // 3. CORS Configuration
    var corsOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        ?? builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? Array.Empty<string>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ApiCorsPolicy", policy =>
        {
            if (corsOrigins.Length > 0)
            {
                policy.WithOrigins(corsOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
            else
            {
                // Development fallback - allow all origins
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
                Log.Warning("CORS is configured to allow all origins. This should only be used in development.");
            }
        });
    });

    // 4. Rate Limiting
    var permitLimit = int.Parse(builder.Configuration["RATE_LIMIT_PERMIT_LIMIT"] ?? builder.Configuration["RateLimiting:PermitLimit"] ?? "100");
    var windowSeconds = int.Parse(builder.Configuration["RATE_LIMIT_WINDOW_SECONDS"] ?? builder.Configuration["RateLimiting:Window"] ?? "60");
    var queueLimit = int.Parse(builder.Configuration["RATE_LIMIT_QUEUE_LIMIT"] ?? builder.Configuration["RateLimiting:QueueLimit"] ?? "10");

    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = permitLimit,
                    QueueLimit = queueLimit,
                    Window = TimeSpan.FromSeconds(windowSeconds)
                }));

        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests. Please try again later.",
                retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) ? retryAfter.TotalSeconds : windowSeconds
            }, cancellationToken: token);

            Log.Warning("Rate limit exceeded for {User} from {IP}",
                context.HttpContext.User.Identity?.Name ?? "Anonymous",
                context.HttpContext.Connection.RemoteIpAddress);
        };
    });

    // 5. Redis
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        try
        {
            var redis = ConnectionMultiplexer.Connect(redisConf);
            Log.Information("Successfully connected to Redis");
            return redis;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to connect to Redis");
            throw;
        }
    });

    // 6. SessionStorage
    builder.Services.AddSingleton<ISessionStorage, RedisSessionStorage>();

    // 7. Polly Retry Policy
    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(Math.Pow(2, retry)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Log.Warning("Retry {RetryCount} after {Delay}s due to {StatusCode}",
                        retryCount, timespan.TotalSeconds, outcome.Result?.StatusCode);
                });
    }

    static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) =>
                {
                    Log.Error("Circuit breaker opened for {Duration}s", duration.TotalSeconds);
                },
                onReset: () =>
                {
                    Log.Information("Circuit breaker reset");
                });
    }

    builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(_ => GetRetryPolicy());

    // 8. HttpClient Factory
    builder.Services.AddHttpClient("ServiceLayer", c =>
    {
        c.BaseAddress = new Uri(slBaseUrl);
        c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

    // 9. ServiceLayerClient
    builder.Services.AddScoped<IServiceLayerClient, ServiceLayerClient>();

    // 10. Controllers with validation
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value?.Errors.Select(x => x.ErrorMessage).ToArray()
                    })
                    .ToList();

                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors,
                    timestamp = DateTime.UtcNow
                });
            };
        });

    builder.Services.AddEndpointsApiExplorer();

    // 11. Swagger/OpenAPI
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SAP Service Layer API Gateway",
            Version = "v1",
            Description = "Secure API Gateway for SAP Business One Service Layer with session management, rate limiting, and resilience patterns",
            Contact = new OpenApiContact
            {
                Name = "API Support",
                Email = "support@yourdomain.com"
            }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments if available
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // 12. Health Checks
    builder.Services.AddHealthChecks()
        .AddRedis(redisConf, name: "redis", tags: new[] { "db", "cache" })
        .AddUrlGroup(new Uri(slBaseUrl), name: "service-layer", tags: new[] { "external", "sap" });

    var app = builder.Build();

    // Middleware Pipeline
    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAP Service Layer API Gateway v1");
            c.RoutePrefix = string.Empty; // Swagger at root
        });
    }
    else
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseCors("ApiCorsPolicy");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");

    Log.Information("API Gateway configured successfully. Starting application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
