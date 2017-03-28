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
        private static readonly object LockObject = new object();

        private static string GetPathForKey(string key)
        {
            return Path.Combine(HttpRuntime.AppDomainAppPath, key);
        }

        public static IEnumerable<Cinema> GetCinemaCache(string key)
        {
            lock (LockObject)
            {
                string cinemasString;
                try
                {
                    cinemasString = File.ReadAllText(GetPathForKey(key));
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

        public static void SetCinemaCache(IEnumerable<Cinema> cinemaList, string key)
        {
            lock (LockObject)
            {
                var cinemaCache = new CinemaFsCache
                {
                    Cinemas = cinemaList,
                    ValidUntil = CacheTimeHelper.NextCzechMidnight
                };

                var cinemaCacheString = JsonConvert.SerializeObject(cinemaCache);
                File.WriteAllText(GetPathForKey(key), cinemaCacheString, Encoding.UTF8);
            }
        }
    }
}