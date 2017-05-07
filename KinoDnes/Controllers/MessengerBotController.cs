using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using KinoDnes.Cache;
using KinoDnes.Models;
using Newtonsoft.Json;
using Serilog;
#pragma warning disable 4014

namespace KinoDnes.Controllers
{
    [Route("webhook")]
    public class MessengerBotController : ApiController
    {
        private readonly string pageToken = ConfigurationManager.AppSettings["pageToken"];

        [HttpGet]
        [AcceptVerbs("GET")]
        public HttpResponseMessage Get(
            [FromUri(Name = "hub.challenge")] string challenge,
            [FromUri(Name = "hub.verify_token")] string verifyToken)
        {
            try
            {
                var acceptedToken = ConfigurationManager.AppSettings["hub.verify_token"];

                if (verifyToken == acceptedToken)
                {
                    Log.Information($"Returning challenge: {challenge}");
                    return new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(challenge, Encoding.UTF8, "text/plain")};
                }
                Log.Warning($"Challenge verification failed. Received verify token: '{verifyToken}'. Expected: '{acceptedToken}'");
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) {Content = new StringContent(e.Message, Encoding.UTF8, "text/plain")};
            }
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        public IHttpActionResult ReceivePost(BotRequest data)
        {
            try
            {
                foreach (var entry in data.entry)
                {
                    foreach (var message in entry.messaging)
                    {
                        if (string.IsNullOrWhiteSpace(message?.message?.text))
                        {
                            continue;
                        }
                        
                        Task.Run(() => RespondToMessage(message.message.text, message.sender.id));
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }
        }

        private async Task RespondToMessage(string request, string senderId)
        {
            var responses = GetResponses(request);
            
            foreach (var message in responses)
            {
                var facebookResponse = await SendMessage(new BotMessageResponse
                {
                    message = new MessageResponse { text = message },
                    recipient = new BotUser { id = senderId }
                });
                var sendResponse = await facebookResponse.Content.ReadAsStringAsync();
                Log.Debug(sendResponse);
            }
        }

        private IEnumerable<string> SplitResponse(string response)
        {
            const int maxMessageSize = 640;
            
            for (int index = 0; index < response.Length; index += maxMessageSize)
            {
                yield return response.Substring(index, Math.Min(maxMessageSize, response.Length - index));
            }
        }

        private async Task<HttpResponseMessage> SendMessage(BotMessageResponse message)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var messageString = JsonConvert.SerializeObject(message);
                return await client.PostAsync($"https://graph.facebook.com/v2.6/me/messages?access_token={pageToken}", new StringContent(messageString, Encoding.UTF8, "application/json"));
            }
        }

        private readonly List<string> allowedDates = new List<string> {"dnes", "zitra"};
        private const string UnknownCommand = "Zadejte mesto a den. Priklad: 'Brno dnes', 'Praha zitra'";

        public IEnumerable<string> GetResponses(string request)
        {
            var requestSplit = request.Split(' ');
            var when = requestSplit.Last().ToLower();
            if (requestSplit.Length < 2 || !allowedDates.Contains(when))
            {
                yield return UnknownCommand;
                yield break;
            }

            var city = string.Join(" ", requestSplit.Take(requestSplit.Length - 1));

            switch (when)
            {
                case "dnes":
                    var responsesToday = ResponseCache.GetAllListingsToday(city);
                    
                    foreach (var cinema in responsesToday)
                    {
                        var split = SplitResponse(cinema.ToString());
                        foreach (var s in split)
                        {
                            yield return s;
                        }
                    }
                    break;
                case "zitra":
                    var responsesTommorow = ResponseCache.GetAllListingsTommorow(city);
                    foreach (var cinema in responsesTommorow)
                    {
                        var split = SplitResponse(cinema.ToString());
                        foreach (var s in split)
                        {
                            yield return s;
                        }
                    }
                    break;
                default:
                    yield return UnknownCommand;
                    break;
            }
        }
    }
}