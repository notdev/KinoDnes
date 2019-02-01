using System;
using System.Collections.Generic;

namespace KinoDnesApi.Model
{
    public class Movie
    {
        public string MovieName { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public IEnumerable<MovieTime> Times { get; set; }

        public string GetRatingClass
        {
            get
            {
                if (Rating < 30) return "shit";
                if (Rating < 70) return "prumer";
                return "dobre";
            }
        }
    }

    public class MovieTime
    {
        public MovieTime(DateTime time, List<string> flags)
        {
            Time = time;
            Flags = flags;
        }

        public DateTime Time { get; }
        public List<string> Flags { get; }
    }
}