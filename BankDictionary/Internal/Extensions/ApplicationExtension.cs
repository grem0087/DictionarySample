using System;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankDictionary.Internal.Extensions
{
    public static class ApplicationExtension
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices
                .GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices
                .GetRequiredService<IOptions<ConsulConfig>>();
            
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();
            
            var uri = new Uri(address);
            var registration = new AgentServiceRegistration()
            {
                ID = $"{consulConfig.Value.ServiceId}-{uri.Port}",
                Name = consulConfig.Value.ServiceName,
                Address = $"{uri.Scheme}://{uri.Host}",
                Port = uri.Port,
                Checks = new []
                {
                    new AgentCheckRegistration()
                    {
                        HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/banks/search",
                        Notes = "Checks /health/status on localhost",
                        Timeout = TimeSpan.FromSeconds(3),
                        Interval = TimeSpan.FromSeconds(5)
                    }
                }
            };
            
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() => {            
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
            return app;
        }
    }
}
