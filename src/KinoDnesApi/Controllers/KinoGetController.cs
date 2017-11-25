using System;
using System.Collections.Generic;
using System.Linq;
using KinoDnesApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace KinoDnesApi.Controllers
{
    public class KinoGetController : Controller
    {
        private readonly IFileSystemShowTimes _fileSystemShowTimes;

        public KinoGetController(IFileSystemShowTimes fileSystemShowTimes)
        {
            _fileSystemShowTimes = fileSystemShowTimes;
        }

        [HttpGet]
        [Route("/api/kino/get/{city}")]
        public IActionResult Get(string city)
        {
            return GetForDate(city, CetTime.Now);
        }

        [HttpGet]
        [Route("/api/kino/get/{city}/{date}")]
        public IActionResult GetForDate(string city, DateTime date)
        {
            return Json(GetListingsForDate(city, date));
        }

        private IEnumerable<Cinema> GetListingsForDate(string city, DateTime date)
        {
            var standardizedCity = StringNormalizer.StandardizeString(city);

            var moviesPlayingAtDate = GetCinemasWithMoviesPlayingAtDate(date);
            var moviesForCity = moviesPlayingAtDate
                                    .Where(c => StringNormalizer.StandardizeString(c.CinemaName).Contains(standardizedCity));
            return moviesForCity;
        }

        private IEnumerable<Cinema> GetCinemasWithMoviesPlayingAtDate(DateTime date)
        {
            IEnumerable<Cinema> cinemas = _fileSystemShowTimes.Get();

            var listingsPlayingSinceDate = new List<Cinema>();

            var endOfDay = date.Date.AddDays(1);

            foreach (var listing in cinemas)
            {
                var moviesInThisListing = new List<Movie>();

                foreach (var movie in listing.Movies)
                {
                    var currentTimes = movie.Times.Where(t => t.Time > date && t.Time < endOfDay).ToList();

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
    }
}