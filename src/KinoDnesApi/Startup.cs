using System;
using Hangfire;
using Hangfire.MemoryStorage;
using KinoDnesApi.DataProviders;
using KinoDnesApi.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using Sentry;

namespace KinoDnesApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICsfdDataProvider, CsfdDataProvider>();
            services.AddSingleton<DataGenerator>();
            services.AddSingleton<IFileSystemShowTimes, FileSystemShowTimes>();
            services.AddCors();
            services.AddSingleton<ISentryClient, SentryClient>();
            services.AddResponseCaching();
            services.AddHangfire(config => config.UseMemoryStorage());
            services.AddMvc(
                    options =>
                    {
                        options.CacheProfiles.Add("Default",
                            new CacheProfile
                            {
                                Duration = 60 * 60
                            });
                    }
                )
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseCors(option => option.AllowAnyOrigin());
            app.UseHangfireServer();
            app.UseHangfireDashboard();
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
            app.UseResponseCaching();

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

            app.UseMvc();
            RecurringJob.AddOrUpdate<ShowTimesUpdater>(u => u.Update(), "0 */2 * * *");
        }
    }
}