using System;
using System.Collections.Generic;
using KinoDnesApi.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace KinoDnesApi
{
    public class RedisShowTimesProvider : IShowTimesProvider
    {
        private readonly IDistributedCache _cache;
        private const string CacheKey = "showtimes";

        public RedisShowTimesProvider(IDistributedCache cache)
        {
            _cache = cache;
        }

        public void Set(IEnumerable<Cinema> cinemas)
        {
            var showTimes = new ShowTimes
            {
                Cinemas = cinemas,
                Created = DateTime.Now
            };
            var serialized = JsonConvert.SerializeObject(showTimes);
            _cache.SetString(CacheKey, serialized);
        }

        public int GetAgeHours()
        {
            var cacheEntry = _cache.GetString(CacheKey);
            if (cacheEntry == null) return int.MaxValue;
            var showtimes = JsonConvert.DeserializeObject<ShowTimes>(cacheEntry);
            var timeSinceLastUpdate = DateTime.Now - showtimes.Created;
            return (int) timeSinceLastUpdate.TotalHours;
        }

        public IEnumerable<Cinema> Get()
        {
            try
            {
                var cacheEntry = _cache.GetString(CacheKey);
                var showtimes = JsonConvert.DeserializeObject<ShowTimes>(cacheEntry);
                return showtimes.Cinemas;
            }
            catch (Exception)
            {
                return new List<Cinema>();
            }
        }
    }
}