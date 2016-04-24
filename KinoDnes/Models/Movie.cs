using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class Movie
    {
        public string MovieName { get; set; }
        public string Url { get; set; }
        public List<string> Times { get; set; }
        public List<string> Flags { get; set; }

    }
}