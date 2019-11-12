using System;
using System.Collections.Generic;

namespace KinoDnesApi.Model
{
    public class ShowTimes
    {
        public DateTime Created { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
    }
}