using System;
using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class CinemaMovie
    {
        public string MovieName { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public List<DateTime> Times { get; set; }
        public List<string> Flags { get; set; }
    }
}