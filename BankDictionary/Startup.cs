using System;
using System.Text;
using BankDictionary.Internal;
using BankDictionary.Internal.Config;
using BankDictionary.Internal.Extensions;
using BankDictionary.Services;
using BankDictionary.Services.Interfaces;
using Database;
using Database.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankDictionary
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<ConsulConfig>(options => Configuration.GetSection("ConsulConfig").Bind(options))
                .Configure<DbConfig>(options => Configuration.GetSection("DbConfig").Bind(options))
                .AddMongo()
                .AddConsul();
            
            services.AddTransient<IBankInfoRepository, BankInfoRepository>();
            services.AddTransient<IBankInfoSearchService, BankInfoSearchService>();
            //services.AddSingleton<IConfiguration>(Configuration);
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseConsul(lifetime);
            app.UseMvc(routes =>
            {
                routes.MapRoute("dispatcher",
                    "{controller}/{action}/{bik?}",
                    "{controller=Banks}/{action=Search}/{bik?}"
                );
            });
        }
    }
}
