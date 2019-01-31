using System;
using System.Collections.Generic;
using System.Linq;
using KinoDnesApi.Model;

namespace KinoDnesApi
{
    public class DataGenerator
    {
        private readonly IFileSystemShowTimes _fileSystemShowTimes;

        public DataGenerator(IFileSystemShowTimes fileSystemShowTimes)
        {
            _fileSystemShowTimes = fileSystemShowTimes;
        }

        private string GetCityName(string cityAndCinemaName)
        {
            var split = cityAndCinemaName.Split(new[] {" - "}, StringSplitOptions.None);
            if (split.Length < 2)
                throw new ArgumentException(
                    $"City name must be in format 'City - Cinema'. Failed to parse '{cityAndCinemaName}'");
            return split[0];
        }

        public IEnumerable<Cinema> GetListingsForDate(string city, DateTime date)
        {
            var standardizedCity = StringNormalizer.StandardizeString(city);

            var moviesPlayingAtDate = GetCinemasWithMoviesPlayingAtDate(date);
            var moviesForCity = moviesPlayingAtDate
                .Where(c => StringNormalizer.StandardizeString(c.CinemaName).Contains(standardizedCity));
            return moviesForCity;
        }

        private IEnumerable<Cinema> GetCinemasWithMoviesPlayingAtDate(DateTime date)
        {
            var cinemas = _fileSystemShowTimes.Get();

            var listingsPlayingSinceDate = new List<Cinema>();

            var endOfDay = date.Date.AddDays(1);

            foreach (var listing in cinemas)
            {
                var moviesInThisListing = new List<Movie>();

                foreach (var movie in listing.Movies)
                {
                    var currentTimes = movie.Times.Where(t => t.Time > date && t.Time < endOfDay).ToList();

                    if (!currentTimes.Any()) continue;

                    var movieWithCurrentTimes = new Movie
                    {
                        MovieName = movie.MovieName,
                        Rating = movie.Rating,
                        Url = movie.Url,
                        Times = currentTimes
                    };

                    moviesInThisListing.Add(movieWithCurrentTimes);
                }

                if (moviesInThisListing.Count > 0)
                    listingsPlayingSinceDate.Add(new Cinema
                    {
                        CinemaName = listing.CinemaName,
                        // Order by first time in list
                        Movies = moviesInThisListing.OrderBy(l => l.Times.First().Time)
                    });
            }

            return listingsPlayingSinceDate;
        }

        public IEnumerable<string> GetCities()
        {
            var allCinemas = GetCinemasWithMoviesPlayingAtDate(CetTime.Now.Date).Select(listing => listing.CinemaName);
            var cities = allCinemas.Select(GetCityName).OrderBy(city => city);

            var topCities = new List<string> {"Praha", "Brno", "Bratislava", "Ostrava"};

            var allCities = topCities.Concat(cities).Distinct();
            return allCities;
        }

        public IEnumerable<Cinema> GetShowtimesForDate(string city, DateTime now)
        {
            throw new NotImplementedException();
        }
    }
}