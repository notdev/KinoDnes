using System;
using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class Movie
    {
        public string MovieName { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public IEnumerable<DateTime> Times { get; set; }
        public IEnumerable<string> Flags { get; set; }
    }
}