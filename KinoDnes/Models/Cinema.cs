using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class Cinema
    {
        public string CinemaName { get; set; }
        public IEnumerable<CinemaMovie> Movies { get; set; }
    }
}