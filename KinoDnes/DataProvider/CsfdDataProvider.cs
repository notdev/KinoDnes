using System.Collections.Generic;
using System.Linq;
using CsfdAPI;
using CsfdAPI.Model;
using KinoDnes.Cache;
using KinoDnes.Models;
using Cinema = KinoDnes.Models.Cinema;
using Movie = KinoDnes.Models.Movie;

namespace KinoDnes.DataProvider
{
    public class CsfdDataProvider : IDataProvider
    {
        public IEnumerable<Cinema> GetAllCinemas()
        {
            var csfdApi = new CsfdApi();
            var allListings = csfdApi.GetAllCinemaListings();

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
                var movieTimes = cinemaListingMovie.Times.Select(time => new MovieTime(time, GetShortFlags(cinemaListingMovie.Flags)));

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

                int rating;
                if (!ratingDictionary.TryGetValue(movie.Url, out rating))
                {
                    var movieInfo = ResponseCache.GetMovieDetails(movie.Url);
                    rating = movieInfo.Rating;
                    ratingDictionary.Add(movie.Url, rating);
                }
                movie.Rating = rating;

                listingMoviesDictionary.Add(movie.MovieName, movie);
            }
            return listingMoviesDictionary.Values;
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