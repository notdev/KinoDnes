using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Serilog;

namespace KinoDnes.Controllers
{
    public class MessengerBotController : ApiController
    {
        [HttpGet]
        [Route("webhook")]
        public IHttpActionResult Get()
        {
            try
            {
                var acceptedToken = ConfigurationManager.AppSettings["hub.verify_token"];

                var querystrings = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                var challenge = querystrings["hub.challenge"];
                var verifyToken = querystrings["hub.verify_token"];

                if (verifyToken == acceptedToken)
                {
                    return Ok(new StringContent(challenge, Encoding.UTF8, "text/plain"));
                }
                return Unauthorized();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }
        }
    }
}