using System.Collections.Generic;
using System.IO;
using System.Text;
using KinoDnes.Models;
using Newtonsoft.Json;
using Serilog;

namespace KinoDnes.Cache
{
    public static class FileSystemCinemaCache
    {
        private static readonly object LockObject = new object();

        private static string CachePath => Path.Combine(Path.GetTempPath(), "cinemacache");

        public static IEnumerable<Cinema> GetCinemaCache()
        {
            lock (LockObject)
            {
                string cinemasString;
                try
                {
                    cinemasString = File.ReadAllText(CachePath);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }

                try
                {
                    var cinemaCache = JsonConvert.DeserializeObject<CinemaFsCache>(cinemasString);
                    if (cinemaCache.ValidUntil > CacheTimeHelper.CurrentCzTime)
                    {
                        Log.Information($"Loaded cinema cache from '{CachePath}'");
                        return cinemaCache.Cinemas;
                    }
                }
                // Return null on deserialization error
                // Data class may change but server will persist cached file with new deployment
                catch (JsonReaderException)
                {
                    return null;
                }
                catch (JsonSerializationException)
                {
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
                    CreatedAt = CacheTimeHelper.CurrentCzTime,
                    ValidUntil = CacheTimeHelper.NextCzechMidnight
                };

                var cinemaCacheString = JsonConvert.SerializeObject(cinemaCache);
                File.WriteAllText(CachePath, cinemaCacheString, Encoding.UTF8);
            }
        }
    }
}