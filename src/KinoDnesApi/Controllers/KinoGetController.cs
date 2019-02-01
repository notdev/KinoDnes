using System;
using System.Collections.Generic;
using KinoDnesApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace KinoDnesApi.Controllers
{
    public class KinoGetController : Controller
    {
        private readonly IFileSystemShowTimes _fileSystemShowTimes;
        private readonly DataGenerator _dataGenerator;

        public KinoGetController(IFileSystemShowTimes fileSystemShowTimes, DataGenerator dataGenerator)
        {
            _fileSystemShowTimes = fileSystemShowTimes;
            _dataGenerator = dataGenerator;
        }

        [HttpGet]
        [Route("api/kino/get/cities")]
        public IEnumerable<string> GetCities()
        {
            return _dataGenerator.GetCities();
        }

        [HttpGet]
        [Route("/api/kino/get/{city}")]
        public IEnumerable<Cinema> GetShowtimesToday(string city)
        {
            return _dataGenerator.GetListingsForDate(city, CetTime.Now);
        }

        [HttpGet]
        [Route("/api/kino/get/showtimesage")]
        public int GetShowtimesAge()
        {
            return _fileSystemShowTimes.GetAgeHours();
        }

        [HttpGet]
        [Route("/api/kino/get/{city}/{date}")]
        public IEnumerable<Cinema> GetShowtimesForDate(string city, DateTime date)
        {
            return _dataGenerator.GetListingsForDate(city, date);
        }
    }
}