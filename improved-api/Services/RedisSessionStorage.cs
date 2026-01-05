using StackExchange.Redis;

namespace ApiGateServiceLayer.Services
{
    /// <summary>
    /// Redis-based session storage for SAP Service Layer cookies
    /// </summary>
    public class RedisSessionStorage : ISessionStorage
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisSessionStorage> _logger;
        private const string SessionKey = "SL_Session_Cookie";
        private static readonly TimeSpan SessionExpiration = TimeSpan.FromMinutes(30);

        public RedisSessionStorage(
            IConnectionMultiplexer redis,
            ILogger<RedisSessionStorage> logger)
        {
            _db = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Store(IEnumerable<string> cookies)
        {
            if (cookies == null || !cookies.Any())
            {
                _logger.LogWarning("Attempted to store null or empty cookies");
                return;
            }

            try
            {
                var cookieString = string.Join("; ", cookies);
                _db.StringSet(SessionKey, cookieString, SessionExpiration);
                _logger.LogDebug("Session cookies stored in Redis with expiration: {Expiration}", SessionExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store session cookies in Redis");
                throw;
            }
        }

        public string Retrieve()
        {
            try
            {
                var value = _db.StringGet(SessionKey);
                
                if (value.IsNullOrEmpty)
                {
                    _logger.LogDebug("No session cookie found in Redis");
                    return string.Empty;
                }

                _logger.LogDebug("Session cookie retrieved from Redis");
                return value.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve session cookies from Redis");
                throw;
            }
        }
    }
}
