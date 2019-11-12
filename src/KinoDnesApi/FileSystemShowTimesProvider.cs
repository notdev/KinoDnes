using System;
using System.Collections.Generic;
using System.IO;
using KinoDnesApi.Model;
using Newtonsoft.Json;

namespace KinoDnesApi
{
    public class FileSystemShowTimesProvider : IShowTimesProvider
    {
        private readonly string _filename;

        public FileSystemShowTimesProvider()
        {
            var tempPath = Path.GetTempPath();
            _filename = Path.Combine(tempPath, "cinemas.json");
        }

        public void Set(IEnumerable<Cinema> cinemas)
        {
            var serialized = JsonConvert.SerializeObject(cinemas);
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
            try
            {
                var jsonText = File.ReadAllText(_filename);
                return JsonConvert.DeserializeObject<IEnumerable<Cinema>>(jsonText);
            }
            catch (IOException)
            {
                return new List<Cinema>();
            }
        }
    }
}