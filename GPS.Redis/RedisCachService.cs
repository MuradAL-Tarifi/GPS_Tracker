using GPS.Redis.Enums;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace GPS.Redis
{
    public interface ICacheService
    {
        Task<string> GetCacheValueAsync(string key);
        Task SetCacheValueAsync(string key, string value, TimeSpan expire);
        Task SetCacheValueAsync(string key, string value, TimeSpan expire, When when);
        Task Publish(PubSubChannel channel, string json);

    }
    public class RedisCachService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisCachService> _logger;

        public RedisCachService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCachService> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public async Task<string> GetCacheValueAsync(string key)
        {
            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                return await db.StringGetAsync(key);
            }
            catch (Exception)
            {

                return string.Empty;
            }

        }

        public async Task SetCacheValueAsync(string key, string value, TimeSpan expire)
        {
            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                await db.StringSetAsync(key, value, expire, When.NotExists, CommandFlags.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }

        public async Task SetCacheValueAsync(string key, string value, TimeSpan expire, When when)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, value, expire, when, CommandFlags.None);
        }

        public async Task Publish(PubSubChannel channel, string json)
        {
            try
            {
                var subscriper = _connectionMultiplexer.GetSubscriber();
                await subscriper.PublishAsync($"ch_{channel}", new RedisValue(json), CommandFlags.FireAndForget);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }


    }
}
