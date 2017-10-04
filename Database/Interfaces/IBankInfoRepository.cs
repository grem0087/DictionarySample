using System.Threading.Tasks;
using Models;

namespace Database.Interfaces
{
    public interface IBankInfoRepository
    {
        Task AddManyAsync(BankInfo[] bankInfo);
        Task<BankInfo> GetAsync(string bik);
        Task<BankInfo[]> FindAsync(string bik);
    }
}
