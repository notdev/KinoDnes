using System;
using KinoDnesApi.DataProviders;
using Microsoft.AspNetCore.Mvc;

namespace KinoDnesApi.Controllers
{
    public class KinoUpdateController : Controller
    {
        private readonly IFileSystemShowTimes _fileSystemShowTimes;
        private readonly ICsfdDataProvider _csfdDataProvider;

        public KinoUpdateController(IFileSystemShowTimes fileSystemShowTimes, ICsfdDataProvider csfdDataProvider)
        {
            _fileSystemShowTimes = fileSystemShowTimes;
            _csfdDataProvider = csfdDataProvider;
        }

        [Route("/api/kino/update")]
        public IActionResult UpdateShowTimes()
        {
            try
            {
                var showTimes = _csfdDataProvider.GetAllShowTimes();
                _fileSystemShowTimes.Set(showTimes);
                return Ok();
            }
            catch (Exception e)
            {
                // TODO logging
                Console.WriteLine(e);
                throw;
            }
        }
    }
}