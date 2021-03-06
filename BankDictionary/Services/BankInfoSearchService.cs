﻿using System;
using System.Threading.Tasks;
using BankDictionary.Services.Interfaces;
using Database.Interfaces;
using Models;

namespace BankDictionary.Services
{
    public class BankInfoSearchService : IBankInfoSearchService
    {
        private readonly IBankInfoRepository _bankInfoRepository;
        private const int MaxBik = 9;

        public BankInfoSearchService(IBankInfoRepository bankInfoRepository)
        {
            _bankInfoRepository = bankInfoRepository;
        }

        public async Task<BankInfo[]> SearchAsync(string bik)
        {            
            BankInfo[] result;
            if (string.IsNullOrEmpty(bik))
            {
                return Array.Empty<BankInfo>();
            }

            if (bik.Length == MaxBik)
            {
                result = new []{ await _bankInfoRepository.GetAsync(bik).ConfigureAwait(false) };
            }
            else
            {
                result = await _bankInfoRepository.FindAsync(bik).ConfigureAwait(false);
            }

            return result;
        }
    }
}
