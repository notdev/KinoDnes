using System;
using System.Collections.Generic;
using System.Linq;
using KinoDnesApi.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KinoDnesApi.Pages
{
    public class CityModel : PageModel
    {
        private readonly DataGenerator _dataGenerator;
        public List<Cinema> CinemaListings { get; set; }
        public string City { get; set; }

        public CityModel(DataGenerator dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        public void OnGet(string city, DateTime? inputDate)
        {
            City = city;

            var date = CetTime.Now;
            if (inputDate.HasValue) date = inputDate.Value;

            NavigationDates = new NavDates(date);

            CinemaListings = _dataGenerator.GetListingsForDate(city, date).ToList();
        }

        public NavDates NavigationDates { get; set; }
    }

    public class NavDates
    {
        public string Selected { get; }
        public string Previous { get; }
        public string Next { get; }

        public NavDates(DateTime selectedDateTime)
        {
            var today = CetTime.Now.Date;
            var isTodaySelected = today.Date == selectedDateTime.Date;
            Previous = isTodaySelected ? string.Empty : FormattedDate(selectedDateTime.Date.AddDays(-1));

            Selected = FormattedDate(selectedDateTime);
            Next = FormattedDate(selectedDateTime.Date.AddDays(1));
        }

        private string FormattedDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}