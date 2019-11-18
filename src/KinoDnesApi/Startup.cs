using System;
using KinoDnesApi.DataProviders;
using KinoDnesApi.Model;
using KinoDnesApi.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Sentry;

namespace KinoDnesApi
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);

            var appSettings = Configuration.Get<AppSettings>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = appSettings.GetRedisSettings();
            });
            services.AddSingleton<ICsfdDataProvider, CsfdDataProvider>();
            services.AddSingleton<DataGenerator>();
            services.AddSingleton<IShowTimesProvider, RedisShowTimesProvider>();
            services.AddCors();
            services.AddSingleton<ISentryClient, SentryClient>();
            services.AddSingleton<DataDogClient>();
            services.AddHostedService<ShowTimesUpdateService>();
            services.AddRazorPages();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseCors(option => option.AllowAnyOrigin());
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 2;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromHours(1)
                    };
                context.Response.Headers[HeaderNames.Vary] =
                    new[] {"Accept-Encoding"};

                await next();
            });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}