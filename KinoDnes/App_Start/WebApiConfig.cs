using System;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;
using Serilog;
using Serilog.Events;

namespace KinoDnes
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}