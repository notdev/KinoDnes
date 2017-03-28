using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using CsfdAPI;
using KinoDnes.DataProvider;
using KinoDnes.Models;
using Movie = CsfdAPI.Model.Movie;

namespace KinoDnes.Cache
{
    public static class ResponseCache
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public static IEnumerable<Cinema> GetAllListingsToday()
        {
            return AddOrGetExisting("today", InitAllCinemaListings);
        }
        public static IEnumerable<Cinema> GetAllListingsTommorow()
        {
            return AddOrGetExisting("tomorrow", InitAllCinemaListingsTommorow);
        }

        public static IEnumerable<string> GetCityList()
        {
            return AddOrGetExisting("cities", InitCityList);
        }

        private static string GetCityName(string cityAndCinemaName)
        {
            var split = cityAndCinemaName.Split(new[] {" - "}, StringSplitOptions.None);
            if (split.Length < 2)
            {
                throw new ArgumentException($"City name must be in format 'City - Cinema'. Failed to parse '{cityAndCinemaName}'");
            }
            return split[0];
        }

        private static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = Cache.AddOrGetExisting(key, newValue, CacheTimeHelper.NextCzechMidnight) as Lazy<T>;
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

        private static IEnumerable<string> InitCityList()
        {
            var allCinemas = GetAllListingsToday().Select(listing => listing.CinemaName);
            var cities = allCinemas.Select(GetCityName).OrderBy(city => city);

            var topCities = new List<string> { "Praha", "Brno", "Bratislava", "Ostrava" };

            var allCities = topCities.Concat(cities).Distinct();
            return allCities;
        }

        public static Movie GetMovieDetails(string url)
        {
            return AddOrGetExisting(url, () => InitMovieDetails(url));
        }

        private static IEnumerable<Cinema> InitAllCinemaListings()
        {
            var listFromFsCache = FileSystemCinemaCache.GetCinemaCache("today");
            if (listFromFsCache != null)
            {
                return listFromFsCache;
            }

            var parser = new CsfdDataProvider();
            var cinemaList = parser.GetAllCinemas().ToList();
            FileSystemCinemaCache.SetCinemaCache(cinemaList, "today");
            return cinemaList;
        }

       private static IEnumerable<Cinema> InitAllCinemaListingsTommorow()
        {
            var listFromFsCache = FileSystemCinemaCache.GetCinemaCache("tomorrow");
            if (listFromFsCache != null)
            {
                return listFromFsCache;
            }

            var parser = new CsfdDataProvider();
            var cinemaList = parser.GetAllCinemasTommorow().ToList();
            FileSystemCinemaCache.SetCinemaCache(cinemaList, "tomorrow");
            return cinemaList;
        }

        private static Movie InitMovieDetails(string url)
        {
            return new CsfdApi().GetMovie(url);
        }
    }
}