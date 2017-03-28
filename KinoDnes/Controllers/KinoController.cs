using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        [HttpGet]
        [Route("api/kino/{city}")]
        public List<Cinema> Get(string city)
        {
            var standardizedCity = StandardizeString(city);

            var listings = ResponseCache.GetAllListingsToday().Where(c => StandardizeString(c.CinemaName).Contains(standardizedCity)).ToList();
            listings = GetCinemasWithMoviesPlayingToday(listings);
            return listings;
        }

        [HttpGet]
        [Route("api/kino/{city}/tomorrow")]
        public List<Cinema> GetTommorow(string city)
        {
            var standardizedCity = StandardizeString(city);
            return ResponseCache.GetAllListingsTommorow().Where(c => StandardizeString(c.CinemaName).Contains(standardizedCity)).ToList();
        }

        [HttpGet]
        [Route("api/kino/Cities")]
        public HttpResponseMessage GetCities()
        {
            return CreateCachedResponse(ResponseCache.GetCityList());
        }

        private HttpResponseMessage CreateCachedResponse(object responseData)
        {
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, responseData);
            responseMessage.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = CacheTimeHelper.SecondsUntilNextMidnight
            };
            return responseMessage;
        }

        private string RemoveDiacritics(string originalString)
        {
            var normalizedString = originalString.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private string StandardizeString(string str)
        {
            var standardizedName = str.ToLower();
            standardizedName = Regex.Replace(standardizedName, @"\s+", "");
            standardizedName = RemoveDiacritics(standardizedName);

            return standardizedName;
        }

        private List<Cinema> GetCinemasWithMoviesPlayingToday(List<Cinema> cinemas)
        {
            var currentTime = CacheTimeHelper.CurrentCzTime;

            var listingsPlayingSinceNow = new List<Cinema>();

            foreach (var listing in cinemas)
            {
                var moviesInThisListing = new List<Movie>();

                foreach (var movie in listing.Movies)
                {
                    var currentTimes = movie.Times.Where(t => t.Time > currentTime).ToList();

                    if (!currentTimes.Any())
                    {
                        continue;
                    }

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
                {
                    listingsPlayingSinceNow.Add(new Cinema
                    {
                        CinemaName = listing.CinemaName,
                        // Order by first time in list
                        Movies = moviesInThisListing.OrderBy(l => l.Times.First().Time)
                    });
                }
            }
            return listingsPlayingSinceNow;
        }
    }
}