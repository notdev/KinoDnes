using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KinoDnes.Models;


namespace KinoDnes
{
    public static class ListingFormatter
    {
        public static string FormatString(IEnumerable<Cinema> listings)
        {
            return string.Join(Environment.NewLine, listings.Select(cinema => cinema.ToString()));
        }
    }
}