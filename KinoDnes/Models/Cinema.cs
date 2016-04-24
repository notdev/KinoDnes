using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class Cinema
    {
        public string CinemaName { get; set; }
        public List<Movie> Movies { get; set; }
    }
}