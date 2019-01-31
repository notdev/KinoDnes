using System;
using System.Collections.Generic;
using System.Linq;
using CsfdAPI;
using CsfdAPI.Model;
using KinoDnesApi.Model;
using Microsoft.Extensions.Caching.Memory;
using Cinema = KinoDnesApi.Model.Cinema;
using Movie = KinoDnesApi.Model.Movie;

namespace KinoDnesApi.DataProviders
{
    public class CsfdDataProvider : ICsfdDataProvider
    {
        private readonly IMemoryCache _cache;
        private readonly CsfdApi _csfdApi = new CsfdApi();

        public CsfdDataProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Cinema> GetAllShowTimes()
        {
            var allListings = _csfdApi.GetAllCinemaListings();

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
            var ratingDictionary = new Dictionary<string, int>();

            foreach (var cinemaListingMovie in cinemaListingMovies)
            {
                var movieTimes = cinemaListingMovie.Times.Select(time => new MovieTime(time, GetShortFlags(cinemaListingMovie.Flags).ToList()));

                // Movie already exist in this listing, add times
                if (listingMoviesDictionary.Keys.Contains(cinemaListingMovie.MovieName))
                {
                    var mergedTimes = listingMoviesDictionary[cinemaListingMovie.MovieName].Times.Concat(movieTimes).OrderBy(t => t.Time);
                    listingMoviesDictionary[cinemaListingMovie.MovieName].Times = mergedTimes;
                    continue;
                }

                var movie = new Movie
                {
                    MovieName = cinemaListingMovie.MovieName,
                    Times = movieTimes,
                    Url = cinemaListingMovie.Url
                };

                if (!ratingDictionary.TryGetValue(movie.Url, out var rating))
                {
                    var movieInfo = GetMovieDetails(movie.Url);
                    rating = movieInfo.Rating;
                    ratingDictionary.Add(movie.Url, rating);
                }
                movie.Rating = rating;

                listingMoviesDictionary.Add(movie.MovieName, movie);
            }
            return listingMoviesDictionary.Values;
        }

        private CsfdAPI.Model.Movie GetMovieDetails(string movieUrl)
        {
            return _cache.GetOrCreate(movieUrl, entry =>
            {
                entry.AbsoluteExpiration = DateTime.Now.AddHours(12);
                return GetMovieWithRetry(movieUrl);
            });
        }

        private CsfdAPI.Model.Movie GetMovieWithRetry(string url)
        {
            var tryCount = 0;
            while (tryCount < 3)
            {
                tryCount++;
                try
                {
                    var movie = _csfdApi.GetMovie(url);
                    return movie;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to get movie, will retry. Exception:{Environment.NewLine}{e}");
                }
            }
            throw new Exception($"Failed to get movie on URL {url}");
        }

        private IEnumerable<string> GetShortFlags(IEnumerable<string> flags)
        {
            foreach (var flag in flags)
            {
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
}