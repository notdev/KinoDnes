using KinoDnesApi.DataProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDnesApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICsfdDataProvider, CsfdDataProvider>();
            services.AddSingleton<IFileSystemShowTimes, FileSystemShowTimes>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}