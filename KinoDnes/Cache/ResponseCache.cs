using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using KinoDnes.Models;
using KinoDnes.Parser;

namespace KinoDnes.Cache
{
    public static class ResponseCache
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static List<Cinema> GetAllListings()
        {
            return AddOrGetExisting("AllCinemasCacheKey", InitAllCinemaListings);
        }

        public static IEnumerable<string> GetCityList()
        {
            return AddOrGetExisting("CityCacheKey", InitCityList);
        }

        private static IEnumerable<string> InitCityList()
        {
            var allCinemas = GetAllListings().Select(l => l.CinemaName).Distinct();
            var cities = allCinemas.Select(cinema => cinema.Split('-').FirstOrDefault()).Distinct();

            var top3Cities = new List<string>() { "Praha", "Brno", "Bratislava", "Ostrava" };
            // Do not show Top cities by population
            var citiesWithoutMainCities = cities.Where(c => !top3Cities.Contains(c));
            return citiesWithoutMainCities;
        }

        public static int GetMovieDetails(string url)
        {
            return AddOrGetExisting(url, () => InitMovieDetails(url));
        }

        private static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = Cache.AddOrGetExisting(key, newValue, DateTime.UtcNow.Date.AddDays(1)) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                Cache.Remove(key);
                throw;
            }
        }

        private static List<Cinema> InitAllCinemaListings()
        {
            var cinemaList = FileSystemCinemaCache.GetCinemaCache();
            if (cinemaList == null)
            {
                var parser = new CsfdKinoParser();
                cinemaList = parser.GetAllCinemas();
                FileSystemCinemaCache.SetCinemaCache(cinemaList);
            }
            return cinemaList;
        }

        private static int InitMovieDetails(string url)
        {
            return new CsfdKinoParser().GetMovieRating(url);
        }
    }
}