using ApiGateServiceLayer.Models;
using Polly;
using Polly.Retry;
using System.Net;

namespace ApiGateServiceLayer.Services
{
    /// <summary>
    /// Client for interacting with SAP Business One Service Layer with automatic session management
    /// </summary>
    public class ServiceLayerClient : IServiceLayerClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly ISessionStorage _storage;
        private readonly ILogger<ServiceLayerClient> _logger;
        private readonly LoginDto _login;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly SemaphoreSlim _loginLock = new(1, 1);

        public ServiceLayerClient(
            IHttpClientFactory factory,
            ISessionStorage storage,
            IConfiguration configuration,
            ILogger<ServiceLayerClient> logger,
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));

            var companyDb = configuration["ServiceLayer:CompanyDB"] 
                ?? throw new InvalidOperationException("ServiceLayer:CompanyDB configuration is missing");
            var userName = configuration["ServiceLayer:UserName"] 
                ?? throw new InvalidOperationException("ServiceLayer:UserName configuration is missing");
            var password = configuration["ServiceLayer:Password"] 
                ?? throw new InvalidOperationException("ServiceLayer:Password configuration is missing");

            _login = new LoginDto
            {
                CompanyDB = companyDb,
                UserName = userName,
                Password = password
            };
        }

        public async Task EnsureSessionAsync()
        {
            var sessionCookie = _storage.Retrieve();
            if (string.IsNullOrEmpty(sessionCookie))
            {
                _logger.LogInformation("No active session found, initiating login");
                var client = _factory.CreateClient("ServiceLayer");
                await LoginAsync(client);
            }
        }

        private async Task LoginAsync(HttpClient client)
        {
            await _loginLock.WaitAsync();
            try
            {
                // Double-check pattern: another thread might have logged in while we were waiting
                var existingSession = _storage.Retrieve();
                if (!string.IsNullOrEmpty(existingSession))
                {
                    _logger.LogDebug("Session already established by another thread");
                    return;
                }

                _logger.LogInformation("Logging in to Service Layer for company: {CompanyDB}", _login.CompanyDB);
                
                var resp = await _retryPolicy.ExecuteAsync(() => client.PostAsJsonAsync("Login", _login));
                resp.EnsureSuccessStatusCode();
                
                if (resp.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    _storage.Store(cookies);
                    _logger.LogInformation("Service Layer session established successfully");
                }
                else
                {
                    _logger.LogWarning("Login response did not contain Set-Cookie headers");
                    throw new InvalidOperationException("Service Layer login did not return session cookies");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to login to Service Layer");
                throw;
            }
            finally
            {
                _loginLock.Release();
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            var client = _factory.CreateClient("ServiceLayer");
            await EnsureSessionAsync();
            
            AttachSessionCookie(client);
            
            var resp = await _retryPolicy.ExecuteAsync(() => client.GetAsync(path));
            
            if (resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Session expired, re-authenticating for GET {Path}", path);
                await LoginAsync(client);
                AttachSessionCookie(client);
                resp = await _retryPolicy.ExecuteAsync(() => client.GetAsync(path));
            }
            
            return resp;
        }

        public async Task<HttpResponseMessage> PostAsync(string path, object payload)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var client = _factory.CreateClient("ServiceLayer");
            await EnsureSessionAsync();
            
            AttachSessionCookie(client);
            
            var resp = await _retryPolicy.ExecuteAsync(() => client.PostAsJsonAsync(path, payload));
            
            if (resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Session expired, re-authenticating for POST {Path}", path);
                await LoginAsync(client);
                AttachSessionCookie(client);
                resp = await _retryPolicy.ExecuteAsync(() => client.PostAsJsonAsync(path, payload));
            }
            
            return resp;
        }

        private void AttachSessionCookie(HttpClient client)
        {
            var sessionCookie = _storage.Retrieve();
            if (string.IsNullOrEmpty(sessionCookie))
            {
                _logger.LogWarning("Attempted to attach session cookie but none was found in storage");
                throw new InvalidOperationException("No session cookie available");
            }

            client.DefaultRequestHeaders.Remove("Cookie");
            client.DefaultRequestHeaders.Add("Cookie", sessionCookie);
        }
    }
}
