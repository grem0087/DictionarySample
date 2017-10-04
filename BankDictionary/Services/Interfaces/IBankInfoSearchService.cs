using System.Threading.Tasks;
using Models;

namespace BankDictionary.Services.Interfaces
{
    public interface IBankInfoSearchService
    {
        Task<BankInfo[]>  SearchAsync(string bik);
    }
}