using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KinoDnesApi.Model;
using Newtonsoft.Json;

namespace KinoDnesApi
{
    public class FileSystemShowTimes : IFileSystemShowTimes
    {
        private const string Filename = "showtimes.json";

        public void Set(IEnumerable<Cinema> showtimes)
        {
            var serialized = JsonConvert.SerializeObject(showtimes);
            File.WriteAllText(Filename, serialized);
        }

        public IEnumerable<Cinema> Get()
        {
            var jsonText = File.ReadAllText(Filename);
            return JsonConvert.DeserializeObject<IEnumerable<Cinema>>(jsonText);
        }
    }
}
