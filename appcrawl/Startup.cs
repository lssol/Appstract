using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using appcrawl.Options;
using appcrawl.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace appcrawl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<MongoOptions>(Configuration.GetSection(MongoOptions.Key));
            services.Configure<RobotOptions>(Configuration.GetSection(RobotOptions.Key));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddSingleton<ApplicationRepository>();
            services.AddSingleton<TemplateRepository>();
            services.AddSingleton<ElementRepository>();
            services.AddMemoryCache();
            
            ConfigureOptions(services);
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(cors =>
            {
                cors
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .SetIsOriginAllowed(h => true);
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
