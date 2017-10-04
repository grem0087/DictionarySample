using System.Threading.Tasks;
using BankDictionary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace BankDictionary.Controllers
{        
    public class BanksController : Controller
    {
        private readonly IBankInfoSearchService _bankInfoSearchService;

        public BanksController(IBankInfoSearchService searcher)
        {
            _bankInfoSearchService = searcher;
        }
        
        [HttpGet]
        public async Task<BankInfo[]> Search(string bik)
        {
            return await _bankInfoSearchService.SearchAsync(bik);            
        }
    }
}
