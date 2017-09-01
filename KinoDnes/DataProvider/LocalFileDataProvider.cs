using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using KinoDnes.Cache;
using KinoDnes.Models;
using Newtonsoft.Json;

namespace KinoDnes.DataProvider
{
    public class LocalFileDataProvider : IDataProvider
    {
        public IEnumerable<Cinema> GetAllCinemas()
        {
            var dataFile = HttpContext.Current.Server.MapPath("~/App_Data/localData.json");

            var cinemaString = File.ReadAllText(dataFile);
            var deserialized = JsonConvert.DeserializeObject<List<Cinema>>(cinemaString);
            if (deserialized == null)
            {
                return new List<Cinema>();
            }

            foreach (var cinema in deserialized)
            {
                foreach (var movie in cinema.Movies)
                {
                    movie.Rating = TryGetRating(movie.Url);
                }
            }

            return deserialized;
        }

        private int TryGetRating(string url)
        {
            try
            {
                if (!url.StartsWith("https://www.csfd"))
                {
                    return 0;
                }

                var csfdMovie = ResponseCache.GetMovieDetails(url);
                return csfdMovie.Rating;
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
    }
}