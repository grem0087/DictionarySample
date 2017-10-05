using System;
using System.Collections.Generic;
using System.Text;
using Database.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Quartz;
using Quartz.Impl;

namespace DataReaper.Internals
{
    public static class ProgramExtensions
    {
        public static IServiceCollection WithMongo(this IServiceCollection services)
        {
            services.AddTransient<IMongoDatabase>((serviceProvider) =>
            {
                var dbConfig = serviceProvider.GetRequiredService<IOptions<DbConfig>>();
                var connectionString = dbConfig.Value.ConnectionString;
                var databaseName = dbConfig.Value.Database;
                return new MongoClient(connectionString).GetDatabase(databaseName);
            });
            return services;
        }

        public static IServiceCollection WithLogging(this IServiceCollection services)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            ILogger logger = loggerFactory.CreateLogger<Program>();

            services.AddSingleton(new LoggerFactory()
                    .AddConsole()
                    .AddDebug())
                    .AddLogging()
                    .AddSingleton(x => logger);

            return services;
        }

        public static IServiceCollection WithSheduler(this IServiceCollection services)
        {
            services
                .AddTransient<IScheduler>(x =>
                {
                    IScheduler sched = new StdSchedulerFactory().GetScheduler().Result;
                    sched.Start();
                    sched.JobFactory = x.GetService<JobFactory>();
                    return sched;
                });

            return services;
        }
    }
}
