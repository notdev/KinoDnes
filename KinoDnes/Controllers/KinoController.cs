using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        [HttpGet]
        [Route("api/kino/{city}")]
        public List<Cinema> Get(string city)
        {
            string standardizedCity = StandardizeString(city);

            var listings = ResponseCache.GetAllListings().Where(c => StandardizeString(c.CinemaName).Contains(standardizedCity)).ToList();
            listings = GetCinemasWithMoviesPlayingToday(listings);
            return listings;
        }

        [HttpGet]
        [Route("api/kino/Cities")]
        public IEnumerable<string> GetCities()
        {
            return ResponseCache.GetCityList();
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
            string standardizedName = str.ToLower();
            standardizedName = Regex.Replace(standardizedName, @"\s+", "");
            standardizedName = RemoveDiacritics(standardizedName);

            return standardizedName;
        }

        private List<Cinema> GetCinemasWithMoviesPlayingToday(List<Cinema> cinemas)
        {
            var currentTime = GetCESTTimeNow();

            var listingsPlayingSinceNow = new List<Cinema>();

            foreach (var listing in cinemas)
            {
                var moviesInThisListing = new List<Movie>();

                foreach (var movie in listing.Movies)
                {
                    var movieWithCurrentTimes = new Movie
                    {
                        MovieName = movie.MovieName,
                        Flags = movie.Flags,
                        Rating = movie.Rating,
                        Url = movie.Url,
                        Times = new List<string>()
                    };

                    foreach (var time in movie.Times)
                    {
                        int hours = int.Parse(time.Split(':').First());
                        int minutes = int.Parse(time.Split(':').Last());

                        var listingTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hours, minutes, 0);

                        if (listingTime > currentTime)
                        {
                            movieWithCurrentTimes.Times.Add($"{hours.ToString("D2")}:{minutes.ToString("D2")}");
                        }
                    }
                    if (movieWithCurrentTimes.Times.Count > 0)
                    {
                        moviesInThisListing.Add(movieWithCurrentTimes);
                    }
                }

                if (moviesInThisListing.Count > 0)
                {
                    listingsPlayingSinceNow.Add(new Cinema
                    {
                        CinemaName = listing.CinemaName,
                        Movies = moviesInThisListing
                    });
                }
            }
            return listingsPlayingSinceNow;
        }

        private DateTime GetCESTTimeNow()
        {
            DateTime cestTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central European Standard Time");
            return cestTime;
        }
    }
}