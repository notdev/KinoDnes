using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        [HttpGet]
        [Route("api/kino/{city}")]
        public List<Cinema> Get(string city)
        {
            var standardizedCity = StandardizeString(city);

            var listings = ResponseCache.GetAllListingsToday()
                .Where(c => StandardizeString(c.CinemaName).Contains(standardizedCity)).ToList();
            return listings;
        }

        [HttpGet]
        [Route("api/kino/{city}/tomorrow")]
        public List<Cinema> GetTommorow(string city)
        {
            var standardizedCity = StandardizeString(city);

            return ResponseCache.GetAllListingsTommorow()
                .Where(c => StandardizeString(c.CinemaName).Contains(standardizedCity)).ToList();
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

        private string RemoveDiacritics(string originalString)
        {
            var normalizedString = originalString.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private string StandardizeString(string str)
        {
            var standardizedName = str.ToLower();
            standardizedName = Regex.Replace(standardizedName, @"\s+", "");
            standardizedName = RemoveDiacritics(standardizedName);

            return standardizedName;
        }

        
    }
}