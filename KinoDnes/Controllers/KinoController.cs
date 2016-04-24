using System.Collections.Generic;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;
using KinoDnes.Parser;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        // GET api/kino
        public List<Cinema> Get(string district)
        {
            var cachedResponse = ResponseCache.Get(district);

            if (cachedResponse != null)
            {
                return (List<Cinema>) cachedResponse;
            }

            var parser = new CsfdKinoParser();
            var listings = parser.GetCinemaListing(district);

            if (listings != null)
            {
                ResponseCache.Set(district, listings);
            }
            return listings;
        }
    }
}