using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KinoDnesApi.Model
{
    public class Cinema
    {
        public string CinemaName { get; set; }
        public IEnumerable<Movie> Movies { get; set; }

        public string CinemaNameWithoutCity => Regex.Replace(CinemaName,".*?- ", "");

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(CinemaName + Environment.NewLine);
            foreach (var movie in Movies)
            {
                builder.Append($"{movie.MovieName} {movie.Rating}% ");
                foreach (var time in movie.Times)
                {
                    builder.Append($"{time.Time:HH:mm} ");
                    if (time.Flags.Any())
                    {
                        builder.Append($"{string.Join(" ", time.Flags.ToList())} ");
                    }
                }
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }
}