using System;
using System.Threading.Tasks;
using Cache.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Cache.Redis.Service
{
    public class RedisCache : ICache
    {
        private readonly IDatabase _cacheDatabase;
        private readonly string _databasePrefix;
        private readonly ConnectionMultiplexer _redisMultiplexer;

        public RedisCache(IConfiguration configuration)
        {
            _databasePrefix = "PASS-Examination-service";// configuration["RedisCluster:DatabasePrefix"];
            _redisMultiplexer = ConnectionMultiplexer.Connect("192.168.1.85:6379, 192.168.1.85:6380, 192.168.1.86:6379, 192.168.1.86:6380, 192.168.1.87:6379, 192.168.1.87:6380, password = theCACHEisonfire");

            _cacheDatabase = _redisMultiplexer.GetDatabase();
        }

        public bool RedisConnected => _redisMultiplexer.IsConnected;

        public async Task<bool> SetCache<T>(string key, T data, TimeSpan? expireTime = null)
        {
            return await _cacheDatabase.StringSetAsync(key, JsonConvert.SerializeObject(data), expireTime);
        }

        public async Task<bool> SetCache(string key, string value, TimeSpan? expireTime = null)
        {
            return await _cacheDatabase.StringSetAsync(key, value, expireTime);
        }

        public async Task<T> GetCache<T>(string key)
        {
            var value = await _cacheDatabase.StringGetAsync(key);
            var valueString = value.ToString();
            return string.IsNullOrEmpty(valueString) ? default : JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public async Task<string> GetCache(string key)
        {
            return await _cacheDatabase.StringGetAsync(key);
        }

        public async Task<bool> DeleteCache(string key)
        {
            return await _cacheDatabase.KeyDeleteAsync(key);
        }
    }
}