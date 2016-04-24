using System;
using System.Runtime.Caching;

namespace KinoDnes.Cache
{
    public static class ResponseCache
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static object Get(string cacheKey)
        {
            var cachedObject = Cache.Get(cacheKey);
            return cachedObject;
        }

        public static void Set(string cacheKey, object itemToCache)
        {
            Cache.Set(cacheKey, itemToCache, DateTime.Today.AddDays(1));
        }
    }
}