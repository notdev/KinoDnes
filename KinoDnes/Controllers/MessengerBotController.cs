using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Serilog;

namespace KinoDnes.Controllers
{
    public class MessengerBotController : ApiController
    {
        [HttpGet]
        [Route("webhook")]
        public string Get()
        {
            try
            {
                var acceptedToken = ConfigurationManager.AppSettings["hub.verify_token"];

                var querystrings = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

                Log.Information(Request.RequestUri.ToString());
                Log.Information(string.Join(Environment.NewLine, querystrings.Select(q => $"{q.Key}:{q.Value}")));

                var challenge = querystrings["hub.challenge"];
                var verifyToken = querystrings["hub.verify_token"];

                if (verifyToken == acceptedToken)
                {
                    Log.Information($"Returning challenge: {challenge}");
                    return challenge;
                }
                Log.Warning($"Challenge verification failed. Received verify token: '{verifyToken}'. Expected: '{acceptedToken}'");
                return "Not authorized";
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return e.Message;
            }
        }
    }
}