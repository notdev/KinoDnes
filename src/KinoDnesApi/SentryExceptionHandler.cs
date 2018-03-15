using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace KinoDnesApi
{
    public class SentryExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ISentryClient _sentryClient;

        public SentryExceptionHandler(RequestDelegate next, ISentryClient sentryClient)
        {
            _next = next;
            _sentryClient = sentryClient;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await _sentryClient.CaptureException(ex);
                throw;
            }
        }
    }
    
    public static class SentryExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseSentryExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SentryExceptionHandler>();
        }
    }
}