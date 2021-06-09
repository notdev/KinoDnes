using System;
using System.Collections.Generic;
using System.Linq;
using CsfdAPI;
using CsfdAPI.Model;
using KinoDnesApi.Model;
using Microsoft.Extensions.Caching.Distributed;
using Cinema = KinoDnesApi.Model.Cinema;
using Movie = KinoDnesApi.Model.Movie;

namespace KinoDnesApi.DataProviders
{
    public class CsfdDataProvider : ICsfdDataProvider
    {
        private readonly IDistributedCache _cache;
        private readonly CsfdApi _csfdApi = new CsfdApi();

        public CsfdDataProvider(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Cinema> GetAllShowTimes()
        {
            var allListings = _csfdApi.GetAllCinemaListings().Result;

            return allListings.Select(ApiListingToListingsWithRating);
        }

        private Cinema ApiListingToListingsWithRating(CsfdAPI.Model.Cinema cinemaListing)
        {
            var listing = new Cinema
            {
                CinemaName = cinemaListing.CinemaName,
                Movies = MergeDuplicateMoviesAndAddRating(cinemaListing.Movies)
            };
            return listing;
        }

        private IEnumerable<Movie> MergeDuplicateMoviesAndAddRating(IEnumerable<CinemaMovie> cinemaListingMovies)
        {
            var listingMoviesDictionary = new Dictionary<string, Movie>();

            foreach (var cinemaListingMovie in cinemaListingMovies)
            {
                var movieTimes = cinemaListingMovie.Times.Select(time =>
                    new MovieTime(time, GetShortFlags(cinemaListingMovie.Flags).ToList()));

                // Movie already exist in this listing, add times
                if (listingMoviesDictionary.Keys.Contains(cinemaListingMovie.MovieName))
                {
                    var mergedTimes = listingMoviesDictionary[cinemaListingMovie.MovieName].Times.Concat(movieTimes)
                        .OrderBy(t => t.Time);
                    listingMoviesDictionary[cinemaListingMovie.MovieName].Times = mergedTimes;
                    continue;
                }

                var movie = new Movie
                {
                    MovieName = cinemaListingMovie.MovieName,
                    Times = movieTimes,
                    Url = cinemaListingMovie.Url
                };

                movie.Rating = GetMovieRating(movie.Url);
                listingMoviesDictionary.Add(movie.MovieName, movie);
            }

            return listingMoviesDictionary.Values;
        }

        private int GetMovieRating(string url)
        {
            try
            {
                var cacheResult = _cache.GetString(url);
                if (cacheResult != null) return int.Parse(cacheResult);

                var movie = _csfdApi.GetMovie(url).Result;
                _cache.SetString(url, movie.Rating.ToString(),
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)});
                return movie.Rating;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to get movie, will retry. Exception:{Environment.NewLine}{e}");
                return 0;
            }
        }

        private IEnumerable<string> GetShortFlags(IEnumerable<string> flags)
        {
            foreach (var flag in flags)
                switch (flag)
                {
                    case "Titulky":
                        yield return "T";
                        break;
                    case "Dabing":
                        yield return "D";
                        break;
                    default:
                        yield return flag;
                        break;
                }
        }
    }
}