using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        [HttpGet]
        [Route("api/kino/{city}")]
        public IEnumerable<Cinema> Get(string city)
        {
            return ResponseCache.GetAllListingsToday(city);
        }

        [HttpGet]
        [Route("api/kino/{city}/tomorrow")]
        public IEnumerable<Cinema> GetTommorow(string city)
        {
            return ResponseCache.GetAllListingsTommorow(city);
        }

        [HttpGet]
        [Route("api/kino/Cities")]
        public HttpResponseMessage GetCities()
        {
            return CreateCachedResponse(ResponseCache.GetCityList());
        }

        private HttpResponseMessage CreateCachedResponse(object responseData)
        {
            var responseMessage = Request.CreateResponse(HttpStatusCode.OK, responseData);
            responseMessage.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = CacheTimeHelper.SecondsUntilNextMidnight
            };
            return responseMessage;
        }
    }
}