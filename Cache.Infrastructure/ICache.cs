using System;
using System.Threading.Tasks;

namespace Cache.Infrastructure
{
    public interface ICache
    {
        Task<bool> SetCache<T>(string key, T data, TimeSpan? expireTime = null);
        Task<bool> SetCache(string key, string value, TimeSpan? expireTime = null);
        Task<T> GetCache<T>(string key);
        Task<string> GetCache(string key);
        Task<bool> DeleteCache(string key);
    }
}