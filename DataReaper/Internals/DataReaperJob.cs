using System;
using System.Threading.Tasks;
using Database.Interfaces;
using DataReaper.Internals.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private ILogger _logger;

        public DataReaperJob(IHttpDataRequest httpDataRequest, IBankInfoRepository bankInfoRepository, IConfiguration configuration, ILogger logger)
        {
            _httpDataRequest = httpDataRequest;
            _bankInfoRepository = bankInfoRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var cbrUrl = _configuration[CbrUrl];
                var resultString = await _httpDataRequest.GetStringAsync(cbrUrl);
                _logger.LogInformation(resultString);
                var resultObject = JsonConvert.DeserializeObject<BankInfo[]>(resultString);
                await _bankInfoRepository.AddManyAsync(resultObject);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }
    }
}