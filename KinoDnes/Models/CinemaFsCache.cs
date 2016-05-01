using System;
using System.Collections.Generic;

namespace KinoDnes.Models
{
    public class CinemaFsCache
    {
        public DateTime ValidUntil { get; set; }
        public List<Cinema> Cinemas { get; set; }
    }
}