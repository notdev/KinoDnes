using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using KinoDnes.Models;
using Newtonsoft.Json;

namespace KinoDnes.Cache
{
    public static class FileSystemCinemaCache
    {
        private static readonly string CacheFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, @"cache.json");

        private static readonly object LockObject = new object();

        public static IEnumerable<Cinema> GetCinemaCache()
        {
            lock (LockObject)
            {
                string cinemasString;
                try
                {
                    cinemasString = File.ReadAllText(CacheFilePath);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }

                try
                {
                    var cinemaCache = JsonConvert.DeserializeObject<CinemaFsCache>(cinemasString);
                    if (cinemaCache.ValidUntil > DateTime.UtcNow)
                    {
                        return cinemaCache.Cinemas;
                    }
                }
                catch (JsonReaderException)
                {
                    // Return null on deserialization error
                    // Data class may change but server will persist cached file with new deployment
                    return null;
                }
            }
            return null;
        }

        public static void SetCinemaCache(IEnumerable<Cinema> cinemaList)
        {
            lock (LockObject)
            {
                var cinemaCache = new CinemaFsCache
                {
                    Cinemas = cinemaList,
                    ValidUntil = DateTime.UtcNow.Date.AddDays(1)
                };

                var cinemaCacheString = JsonConvert.SerializeObject(cinemaCache);
                File.WriteAllText(CacheFilePath, cinemaCacheString, Encoding.UTF8);
            }
        }
    }
}