using System.Collections.Generic;
using System.Linq;
using CsfdAPI;
using CsfdAPI.Model;
using KinoDnes.Cache;
using Cinema = KinoDnes.Models.Cinema;
using Movie = KinoDnes.Models.Movie;

namespace KinoDnes.DataProvider
{
    public class CsfdDataProvider
    {
        public IEnumerable<Cinema> GetAllCinemas()
        {
            var csfdApi = new CsfdApi();
            var allListings = csfdApi.GetAllCinemaListings().ToList();

            var allCinemaList = new List<Cinema>();
            allListings.ForEach(listing => { allCinemaList.Add(ApiListingToListingsWithRating(listing)); });

            return allCinemaList;
        }

        private Cinema ApiListingToListingsWithRating(CsfdAPI.Model.Cinema cinemaListing)
        {
            var listing = new Cinema
            {
                CinemaName = cinemaListing.CinemaName,
                Movies = ApiMoviesToMoviesWithRating(cinemaListing.Movies)
            };
            return listing;
        }

        private IEnumerable<Movie> ApiMoviesToMoviesWithRating(IEnumerable<CinemaMovie> cinemaListingMovies)
        {
            var listingMovies = new List<Movie>();
            var ratingDictionary = new Dictionary<string, int>();

            foreach (var cinemaListingMovie in cinemaListingMovies)
            {
                var movie = new Movie
                {
                    MovieName = cinemaListingMovie.MovieName,
                    Flags = cinemaListingMovie.Flags,
                    Times = cinemaListingMovie.Times,
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

                listingMovies.Add(movie);
            }
            return listingMovies;
        }
    }
}