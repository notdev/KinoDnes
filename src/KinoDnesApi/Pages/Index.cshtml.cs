using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KinoDnesApi.Pages
{
    public class IndexModel : PageModel
    {
        public string City { get; set; }
        public DateTime? Date { get; set; }


        public void OnGet(string city, DateTime? date)
        {
            City = city;
            Date = date;
        }
    }
}