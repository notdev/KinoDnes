using System;
using System.Collections.Generic;
using KinoDnesApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace KinoDnesApi.Controllers
{
    public class KinoGetController : Controller
    {
        private readonly IShowTimesProvider _showTimesProvider;
        private readonly DataGenerator _dataGenerator;

        public KinoGetController(IShowTimesProvider showTimesProvider, DataGenerator dataGenerator)
        {
            _showTimesProvider = showTimesProvider;
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
            return _showTimesProvider.GetAgeHours();
        }

        [HttpGet]
        [Route("/api/kino/get/{city}/{inputDate}")]
        public IEnumerable<Cinema> GetShowtimesForDate(string city, DateTime inputDate)
        {
            return _dataGenerator.GetListingsForDate(city, inputDate);
        }
    }
}