using System;
using Consul;
using Database.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankDictionary.Internal.Extensions
{
    public static class SerivicesExtensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {            
            services.AddTransient<IMongoDatabase>((serviceProvider) =>
            {
                var dbConfig = serviceProvider.GetRequiredService<IOptions<DbConfig>>();
                return new MongoClient(dbConfig.Value.ConnectionString).GetDatabase(dbConfig.Value.Database);
            });

            return services;
        }

        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var config = p.GetRequiredService<IOptions<ConsulConfig>>();
                consulConfig.Address = new Uri(config.Value.Address);
            }));

            return services;
        }
    }
}
