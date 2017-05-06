﻿using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using KinoDnes.Models;
using Newtonsoft.Json;
using Serilog;

namespace KinoDnes.Controllers
{
    [Route("webhook")]
    public partial class MessengerBotController : ApiController
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
        public async Task<IHttpActionResult> ReceivePost(BotRequest data)
        {
            foreach (var entry in data.entry)
            {
                foreach (var message in entry.messaging)
                {
                    if (string.IsNullOrWhiteSpace(message?.message?.text))
                    {
                        continue;
                    }

                    var msg = "You said: " + message.message.text;
                    await SendMessage(new FacebookMessage(msg, message.sender.id));
                }
            }

            return Ok();
        }

        private async Task SendMessage(FacebookMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var messageString = JsonConvert.SerializeObject(message);
                await client.PostAsync($"https://graph.facebook.com/v2.6/me/messages?access_token={pageToken}", new StringContent(messageString, Encoding.UTF8, "application/json"));
            }
        }
    }
}