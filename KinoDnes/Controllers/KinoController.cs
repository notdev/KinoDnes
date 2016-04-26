using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using KinoDnes.Models;

namespace KinoDnes.Controllers
{
    public class KinoController : ApiController
    {
        // GET api/kino
        public List<Cinema> Get(string city)
        {
            if (city != "Brno")
            {
                return null;
            }
            var serializer = new JavaScriptSerializer();
            var savedListings =  File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, "savedData.json"));
            var listings = serializer.Deserialize<List<Cinema>>(savedListings);
            return listings;
        }
    }
}