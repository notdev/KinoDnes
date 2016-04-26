using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        // GET api/kino
        public List<Cinema> Get(string city)
        {
            var listings = ResponseCache.GetAllListings().Where(c => c.CinemaName.Contains(city)).ToList();
            return listings;
        }
    }
}