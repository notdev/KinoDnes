using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;
using KinoDnes.Parser;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        // GET api/kino
        public List<Cinema> Get(string city)
        {
            var cachedResponse = ResponseCache.Get(city);

            if (cachedResponse != null)
            {
                return (List<Cinema>) cachedResponse;
            }

            var parser = new CsfdKinoParser();
            var listings = parser.GetAllCinemas().Where(c => c.CinemaName.Contains(city)).ToList();

            ResponseCache.Set(city, listings);
            return listings;
        }
    }
}