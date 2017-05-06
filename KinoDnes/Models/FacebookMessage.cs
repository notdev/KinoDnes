using System.Web.Http;

namespace KinoDnes.Controllers
{
    public partial class MessengerBotController : ApiController
    {
        public class FacebookMessage
        {
            public string Message { get; }
            public string Sender { get; }

            public FacebookMessage(string message, string sender)
            {
                Message = message;
                Sender = sender;
            }
        }
    }
}