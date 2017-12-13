using System;
using System.Collections.Generic;
using System.IO;
using KinoDnesApi.Model;
using Newtonsoft.Json;

namespace KinoDnesApi
{
    public class FileSystemShowTimes : IFileSystemShowTimes
    {
        private readonly string _filename;

        public FileSystemShowTimes()
        {
            var tempPath = Path.GetTempPath();
            _filename = Path.Combine(tempPath, "showtimes.json");
        }

        public void Set(IEnumerable<Cinema> showtimes)
        {
            var serialized = JsonConvert.SerializeObject(showtimes);
            File.WriteAllText(_filename, serialized);
        }

        public int GetAgeHours()
        {
            var created = File.GetLastWriteTime(_filename);
            var now = DateTime.Now;
            var timeSinceLastUpdate = now - created;
            return (int) timeSinceLastUpdate.TotalHours;
        }

        public IEnumerable<Cinema> Get()
        {
            var jsonText = File.ReadAllText(_filename);
            return JsonConvert.DeserializeObject<IEnumerable<Cinema>>(jsonText);
        }
    }
}