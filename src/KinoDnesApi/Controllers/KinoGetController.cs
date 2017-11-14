using Microsoft.AspNetCore.Mvc;

namespace KinoDnesApi.Controllers
{
    public class KinoGetController : Controller
    {
        [HttpGet]
        [Route("/api/kino/hello")]
        public IActionResult Get()
        {
            return Ok("Hello");
        }
    }
}