﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;
using KinoDnes.Time;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        [HttpGet]
        [Route("api/kino/{city}")]
        public List<Cinema> Get(string city)
        {
            var standardizedCity = StandardizeString(city);

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
            var standardizedName = str.ToLower();
            standardizedName = Regex.Replace(standardizedName, @"\s+", "");
            standardizedName = RemoveDiacritics(standardizedName);

            return standardizedName;
        }

        private List<Cinema> GetCinemasWithMoviesPlayingToday(List<Cinema> cinemas)
        {
            var currentTime = TimeHelper.CESTTimeNow;

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
                        Times = new List<DateTime>()
                    };

                    foreach (var time in movie.Times)
                    {
                        if (time > currentTime)
                        {
                            movieWithCurrentTimes.Times.Add(time);
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
                        Movies = moviesInThisListing.OrderBy(l => l.Times.First())
                    });
                }
            }
            return listingsPlayingSinceNow;
        }
    }
}