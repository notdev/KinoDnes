using System;
using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class Movie
    {
        public string MovieName { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public IEnumerable<MovieTime> Times { get; set; }
    }

    public class MovieTime
    {
        public MovieTime(DateTime time, IEnumerable<string> flags)
        {
            Time = time;
            Flags = flags;
        }
        public DateTime Time { get; }
        public IEnumerable<string> Flags { get; }
    }
}