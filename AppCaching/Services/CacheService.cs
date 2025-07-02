using AppCaching.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppCaching.Services
{
    /// <summary>
    /// ICacheService defines the contract for cache operations, allowing
    /// </summary>
    public interface ICacheService
    {
        Task Set<T>(string key, T value, TimeSpan? expirationTime = null);
        Task<T?> GetAsync<T>(string key);
    }
    public class CacheService : ICacheService
    {
        /// <summary>
        /// CacheService is responsible for managing cache operations 
        /// using either InMemory or Redis cache based on configuration settings.
        /// </summary>
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _redisCache;
        private readonly CacheType _cacheType;
        private readonly TimeSpan _defaultExpiration;
        private readonly IConfiguration _config;
        public CacheService(IMemoryCache memoryCache, IDistributedCache redisCache, IConfiguration config)
        {
            _memoryCache = memoryCache;
            _redisCache = redisCache;
            _config = config;

            //Determine cache type based on configuration settings
            //Default to InMemory if not specified
            _cacheType = _config["CacheSettings:CacheType"] == "Redis" ? CacheType.Redis : CacheType.InMemory;
            _defaultExpiration = TimeSpan.FromMinutes(5); // Default expiration time
            if (int.TryParse(_config["CacheSettings:ExpirationTime"], out int minutes))
                _defaultExpiration = TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// sets a value in the cache with an optional expiration time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        /// <returns></returns>
        public async Task Set<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            if(_cacheType == CacheType.Redis)
                await SetRedisAsync(key, value, expirationTime);
            else
                SetInMemory(key, value, expirationTime);
        }

        /// <summary>
        /// Retrieves a value from the cache by its key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            if (_cacheType == CacheType.Redis)
                return await GetFromRedisAsync<T>(key);
            else
                return GetFromMemory<T>(key);
        }

        /// <summary>
        /// Sets a value in memory cache with an optional expiration time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void SetInMemory<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            _memoryCache.Set(key, value, expirationTime ?? _defaultExpiration);
        }

        /// <summary>
        /// gets a value from memory cache by its key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private T? GetFromMemory<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if (_memoryCache.TryGetValue(key, out var value))
            {
                return value is T typedValue ? typedValue : default;
            }

            return default;
        }

        /// <summary>
        /// Sets a value in Redis cache with an optional expiration time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task SetRedisAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime ?? _defaultExpiration
            };

            var serializedData = JsonSerializer.SerializeToUtf8Bytes(value);
            await _redisCache.SetAsync(key, serializedData, options);
        }

        /// <summary>
        /// Gets a value from Redis cache by its key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<T?> GetFromRedisAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            var data = await _redisCache.GetAsync(key);

            if (data == null || data.Length == 0)
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(data);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}
