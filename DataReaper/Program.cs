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
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DataReaper
{
    class Program
    {
        private const string DelayMinutesConfigName = "DelayMinutes";
        public static IConfiguration Configuration;
        public static IServiceProvider ServiceProvider;
        public static IScheduler Scheduler;

        static void Main(string[] args)
        {
            StartConfigurationBuilder();
            StartServiceProvider();
            InitializeSceduler();
            Scheduler.Start();

            Console.ReadLine();
        }

        private static void InitializeSceduler()
        {
            var delayMinutesString = Configuration[DelayMinutesConfigName];
            int.TryParse(delayMinutesString, out int delayMinutes);
            Scheduler = ServiceProvider.GetService<IScheduler>();
            Scheduler.ScheduleJob(
                JobBuilder.Create<IJob>().Build(),
                TriggerBuilder.Create().WithSimpleSchedule(s => s.WithIntervalInMinutes(delayMinutes).RepeatForever()).Build());            
        }

        private static void StartConfigurationBuilder()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        private static void StartServiceProvider()
        {
            ServiceProvider = new ServiceCollection()
                .Configure<DbConfig>(options => Configuration.GetSection("DbConfig").Bind(options))
                .WithLogging()
                .AddTransient<IBankInfoRepository, BankInfoRepository>()
                .WithMongo()
                .AddTransient<IHttpDataRequest, HttpDataRequest>()
                .AddSingleton<IConfiguration>(Configuration)
                .AddTransient<IJob, DataReaperJob>()
                .AddTransient<JobFactory>(x => new JobFactory(x))
                .WithSheduler()
                .BuildServiceProvider();
        }
    }
}