﻿using System;
using System.Threading.Tasks;
using Database.Interfaces;
using DataReaper.Internals.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Models;
using Newtonsoft.Json;
using Quartz;

namespace DataReaper.Internals
{
    public class DataReaperJob : IJob
    {
        private readonly IBankInfoRepository _bankInfoRepository;
        private readonly IHttpDataRequest _httpDataRequest;
        private readonly IConfiguration _configuration;
        private const string CbrUrl = "CbrUrl";

        public DataReaperJob(IHttpDataRequest httpDataRequest, IBankInfoRepository bankInfoRepository, IConfiguration configuration)
        {
            _httpDataRequest = httpDataRequest;
            _bankInfoRepository = bankInfoRepository;
            _configuration = configuration;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            var cbrUrl = _configuration[CbrUrl];            
            var resultString = await _httpDataRequest.GetStringAsync(cbrUrl);
            Console.WriteLine(resultString);
            var resultObject = JsonConvert.DeserializeObject<BankInfo[]>(resultString);
            await _bankInfoRepository.AddManyAsync(resultObject);
        }
    }
}
