using System;
using System.Linq;
using System.Threading.Tasks;
using KinoDnesApi.DataProviders;
using KinoDnesApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KinoDnesApi.Controllers
{
    public class KinoUpdateController : Controller
    {
        private readonly IFileSystemShowTimes _fileSystemShowTimes;
        private readonly ICsfdDataProvider _csfdDataProvider;
        private readonly string _apiKey;

        public KinoUpdateController(IFileSystemShowTimes fileSystemShowTimes, ICsfdDataProvider csfdDataProvider, IOptions<EnvironmentVariables> configuration)
        {
            var configurationValue = configuration.Value;
            _apiKey = configurationValue.ApiKey;

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentException("'ApiKey' must be defined as environment variable");
            }

            _fileSystemShowTimes = fileSystemShowTimes;
            _csfdDataProvider = csfdDataProvider;
        }

        private void UpdateShowTimes()
        {
            var showTimes = _csfdDataProvider.GetAllShowTimes().ToList();
            if (showTimes.Any())
            {
                _fileSystemShowTimes.Set(showTimes);
            }
            else
            {
                throw new Exception("Failed to get any showtimes");
            }
        }

        [Route("/api/kino/update")]
        public IActionResult UpdateShowTimes(string apiKey)
        {
            if (apiKey != _apiKey)
            {
                return Unauthorized();
            }

            Task.Run(() => UpdateShowTimes());
            return Ok();
        }
    }
}