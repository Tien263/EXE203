using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Exe_Demo.Services
{
    /// <summary>
    /// In-Memory Cache Service Implementation
    /// Optimizes performance by caching frequently accessed data
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _keys = new ConcurrentDictionary<string, byte>();
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                _cache.TryGetValue(key, out T? value);
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache key: {Key}", key);
                return Task.FromResult<T?>(default);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };

                _cache.Set(key, value, options);
                _keys.TryAdd(key, 0);
                
                _logger.LogDebug("Cache set for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _keys.TryRemove(key, out _);
                
                _logger.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache key: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            try
            {
                var keysToRemove = _keys.Keys.Where(k => k.StartsWith(prefix)).ToList();
                
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    _keys.TryRemove(key, out _);
                }
                
                _logger.LogDebug("Cache removed for prefix: {Prefix}, Count: {Count}", prefix, keysToRemove.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by prefix: {Prefix}", prefix);
            }

            return Task.CompletedTask;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            var value = await factory();
            await SetAsync(key, value, expiration);
            
            return value;
        }
    }
}
