using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using KinoDnes.Cache;
using KinoDnes.Models;
using KinoDnes.Time;

namespace KinoDnes.Parser
{
    public class CsfdKinoParser
    {
        public List<Cinema> GetAllCinemas()
        {
            var allCinemaList = new List<Cinema>();

            // CZ
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-1/?district-filter=0"));
            // SK
            allCinemaList.AddRange(GetCinemaListing("http://www.csfd.cz/kino/filtr-2/?district-filter=0"));

            GetMovieDetails(allCinemaList);

            return allCinemaList;
        }

        private void GetMovieDetails(List<Cinema> cinemaList)
        {
            var ratingDictionary = new Dictionary<string, int>();

            foreach (var cinema in cinemaList)
            {
                foreach (var movie in cinema.Movies)
                {
                    int rating;
                    if (!ratingDictionary.TryGetValue(movie.Url, out rating))
                    {
                        rating = ResponseCache.GetMovieDetails(movie.Url);
                        ratingDictionary.Add(movie.Url, rating);
                    }
                    movie.Rating = rating;
                }
            }
        }

        private List<Cinema> GetCinemaListing(string url)
        {
            var document = GetDocumentByUrl(url);

            var cinemaElements = GetCinemaElements(document);

            var cinemaListing = new List<Cinema>();

            foreach (var cinema in cinemaElements)
            {
                var titleNode = cinema.SelectSingleNode("div/h2");
                var title = titleNode.InnerText;

                var movieNodes = GetMovieElements(cinema);
                var movieList = movieNodes.Select(GetMovie).ToList();

                cinemaListing.Add(new Cinema
                {
                    CinemaName = title,
                    Movies = movieList
                });
            }

            return cinemaListing;
        }

        private HtmlNodeCollection GetCinemaElements(HtmlDocument doc)
        {
            var result = doc.DocumentNode.SelectNodes("//*[contains(@class,'cinema')]");
            return result;
        }

        private HtmlNodeCollection GetMovieElements(HtmlNode cinemaNode)
        {
            var result = cinemaNode.SelectNodes("div/table/tr");
            return result;
        }

        private Movie GetMovie(HtmlNode movieNode)
        {
            var titleNode = movieNode.SelectSingleNode("th/a");

            var title = titleNode.InnerText;
            var url = $"http://csfd.cz{titleNode.GetAttributeValue("href", "")}";
            var yearNode = movieNode.SelectSingleNode("th/span");
            var year = yearNode.InnerText;

            var timeNodes = movieNode.SelectNodes("td[not(@class)]");

            var timeList = GetTimeList(timeNodes);

            var flags = GetFlags(movieNode);

            return new Movie
            {
                MovieName = $"{title} {year}",
                Times = timeList,
                Url = url,
                Flags = flags
            };
        }

        private List<DateTime> GetTimeList(HtmlNodeCollection timeNodes)
        {
            var timesAsString = from timeNode in timeNodes where !string.IsNullOrEmpty(timeNode.InnerText) select timeNode.InnerText;
            return timesAsString.Select(GetTimeFromString).ToList();
        }

        private DateTime GetTimeFromString(string time)
        {
            var cestNow = TimeHelper.CESTTimeNow;
            var hoursAndMinutes = time.Split(':');
            string hourString = hoursAndMinutes.First();
            string minutesString = hoursAndMinutes.Last();
            int hours = int.Parse(hourString);
            int minutes = int.Parse(minutesString);
            var timeFromString = new DateTime(cestNow.Year, cestNow.Month, cestNow.Day, hours, minutes, 0);
            return timeFromString;
        }

        private List<string> GetFlags(HtmlNode movieNode)
        {
            var flagNodes = movieNode.SelectNodes("td[@class='flags']/span");

            var flagList = new List<string>();
            if (flagNodes != null)
            {
                foreach (var flagNode in flagNodes)
                {
                    string flag = string.Empty;

                    switch (flagNode.InnerText)
                    {
                        case "D":
                            flag = "Dabing";
                            break;
                        case "T":
                            flag = "Titulky";
                            break;
                        case "3D":
                            flag = "3D";
                            break;
                    }
                    flagList.Add(flag);
                }
            }

            return flagList;
        }

        public int GetMovieRating(string url)
        {
            int rating;

            HtmlDocument document = GetDocumentByUrl(url);

            var node = document.DocumentNode.SelectSingleNode("//h2[@class='average']");

            try
            {
                rating = int.Parse(Regex.Match(node.InnerText, @"\d*").Value);
            }
            catch
            {
                rating = -1;
            }

            return rating;
        }

        /// <summary>
        ///     Get HTMLDocument by URL
        /// </summary>
        /// <param name="url">URL to load HTML from</param>
        /// <returns>HtmlDocument instance</returns>
        private HtmlDocument GetDocumentByUrl(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    var responseString = reader.ReadToEnd();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseString);
                    return htmlDocument;
                }
            }
            return null;
        }
    }
}