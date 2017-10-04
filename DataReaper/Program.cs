using System;
using System.IO;
using Database;
using Database.Interfaces;
using DataReaper.Internals;
using DataReaper.Internals.Services;
using DataReaper.Internals.Services.Interfaces;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DataReaper
{
    class Program
    {
        private const string DelayMinutesConfigName = "DelayMinutes";
        public static IConfiguration Configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            
            var serviceProvider = new ServiceCollection()
                .AddTransient<IBankInfoRepository, BankInfoRepository>()
                .AddTransient<IMongoDatabase>((x) =>
                {
                    var connectionString = Configuration["ConnectionString"];
                    var databaseName = Configuration["Database"];
                    return new MongoClient(connectionString).GetDatabase(databaseName);
                })
                .AddTransient<IHttpDataRequest, HttpDataRequest>()
                .AddSingleton<IConfiguration>(Configuration)
                .AddTransient<IJob, DataReaperJob>()
                .AddTransient<JobFactory>(x => new JobFactory(x))
                .AddTransient<IScheduler>(x =>
                {
                    IScheduler sched = new StdSchedulerFactory().GetScheduler().Result;
                    sched.Start();
                    sched.JobFactory = x.GetService<JobFactory>();
                    return sched;
                }).BuildServiceProvider();

            var delayMinutesString = Configuration[DelayMinutesConfigName];
            int.TryParse(delayMinutesString, out int delayMinutes);
            var scheduler = serviceProvider.GetService<IScheduler>();            
            scheduler.ScheduleJob(
                JobBuilder.Create<IJob>().Build(),
                TriggerBuilder.Create().WithSimpleSchedule(s => s.WithIntervalInMinutes(delayMinutes).RepeatForever()).Build());
            scheduler.Start();
            Console.WriteLine("Application started...");
            Console.ReadLine();
        }
    }
}