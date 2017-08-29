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

        public static IEnumerable<Cinema> GetAllListingsToday(string city)
        {
            var standardizedCity = StringNormalizer.StandardizeString(city);
            return AddOrGetExisting($"today{city}", () =>
            {
                return GetAllListingsToday()
                    .Where(c => StringNormalizer.StandardizeString(c.CinemaName).Contains(standardizedCity));
            }, DateTime.Now.AddMinutes(10));
        }

        public static IEnumerable<Cinema> GetAllListingsTommorow(string city)
        {
            var standardizedCity = StringNormalizer.StandardizeString(city);
            return AddOrGetExisting($"tomorrow{city}", () =>
            {
                return GetAllListingsTommorow()
                    .Where(c => StringNormalizer.StandardizeString(c.CinemaName).Contains(standardizedCity));
            });
        }

        public static IEnumerable<Cinema> GetListingsForDate(string city, DateTime date)
        {
            var standardizedCity = StringNormalizer.StandardizeString(city);
            return AddOrGetExisting($"{date.Date}{city}", () =>
            {
                return GetAllListingsForDate(date)
                    .Where(c => StringNormalizer.StandardizeString(c.CinemaName).Contains(standardizedCity));
            });
        }

        private static IEnumerable<Cinema> GetAllListingsToday()
        {
            return AddOrGetExisting("today", () => GetCinemasWithMoviesPlayingAtDate(GetAllListings(), CacheTimeHelper.CurrentCzTime), DateTime.Now.AddMinutes(10));
        }

        private static IEnumerable<Cinema> GetAllListingsTommorow()
        {
            return GetAllListingsForDate(CacheTimeHelper.NextCzechMidnight);
        }

        private static IEnumerable<Cinema> GetAllListingsForDate(DateTime date)
        {
            var key = date.ToString("ddMMyyyy");
            return AddOrGetExisting(key, () => GetCinemasWithMoviesPlayingAtDate(GetAllListings(), date));
        }

        private static IEnumerable<Cinema> GetCinemasWithMoviesPlayingAtDate(IEnumerable<Cinema> cinemas, DateTime date)
        {
            var listingsPlayingSinceDate = new List<Cinema>();
            
            var endOfDay = date.Date.AddDays(1);

            foreach (var listing in cinemas)
            {
                var moviesInThisListing = new List<Models.Movie>();

                foreach (var movie in listing.Movies)
                {
                    var currentTimes = movie.Times.Where(t => t.Time > date && t.Time < endOfDay).ToList();

                    if (!currentTimes.Any())
                    {
                        continue;
                    }

                    var movieWithCurrentTimes = new Models.Movie
                    {
                        MovieName = movie.MovieName,
                        Rating = movie.Rating,
                        Url = movie.Url,
                        Times = currentTimes
                    };

                    moviesInThisListing.Add(movieWithCurrentTimes);
                }

                if (moviesInThisListing.Count > 0)
                {
                    listingsPlayingSinceDate.Add(new Cinema
                    {
                        CinemaName = listing.CinemaName,
                        // Order by first time in list
                        Movies = moviesInThisListing.OrderBy(l => l.Times.First().Time)
                    });
                }
            }
            return listingsPlayingSinceDate;
        }

        private static IEnumerable<Cinema> GetAllListings()
        {
            return AddOrGetExisting("all", InitAllCinemaListings);
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

        private static T AddOrGetExisting<T>(string key, Func<T> valueFactory, DateTimeOffset validUntil)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = Cache.AddOrGetExisting(key, newValue, validUntil) as Lazy<T>;
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

        private static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        {
            return AddOrGetExisting(key, valueFactory, CacheTimeHelper.NextCzechMidnight);
        }

        private static IEnumerable<string> InitCityList()
        {
            var allCinemas = GetAllListingsToday().Select(listing => listing.CinemaName);
            var cities = allCinemas.Select(GetCityName).OrderBy(city => city);

            var topCities = new List<string> {"Praha", "Brno", "Bratislava", "Ostrava"};

            var allCities = topCities.Concat(cities).Distinct();
            return allCities;
        }

        private static readonly CsfdApi CsfdApi = new CsfdApi();

        public static Movie GetMovieDetails(string url)
        {
            return AddOrGetExisting(url, () => CsfdApi.GetMovie(url));
        }

        private static IEnumerable<Cinema> InitAllCinemaListings()
        {
            var listFromFsCache = FileSystemCinemaCache.GetCinemaCache();
            if (listFromFsCache != null)
            {
                return listFromFsCache;
            }

            var csfdDataProvider = new CsfdDataProvider();
            var cinemaList = csfdDataProvider.GetAllCinemas().ToList();
            FileSystemCinemaCache.SetCinemaCache(cinemaList);
            return cinemaList;
        }
    }
}