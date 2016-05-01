using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        // GET api/kino
        public List<Cinema> Get(string city)
        {
            string standardizedCity = StandardizeString(city);

            var listings = ResponseCache.GetAllListings().Where(c => StandardizeString(c.CinemaName).Contains(standardizedCity)).ToList();
            return listings;
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
            string standardizedName = str.ToLower();
            standardizedName = Regex.Replace(standardizedName, @"\s+", "");
            standardizedName = RemoveDiacritics(standardizedName);

            return standardizedName;
        }
    }
}