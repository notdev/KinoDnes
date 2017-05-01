using System;
using System.IO;
using System.Web;
using System.Web.Http;
using Serilog;
using Serilog.Events;

namespace KinoDnes
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kinodnes.log");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(a => a.File(logFile, LogEventLevel.Debug, fileSizeLimitBytes: 50000000))
                .CreateLogger();
            Log.Information("Application started");
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}