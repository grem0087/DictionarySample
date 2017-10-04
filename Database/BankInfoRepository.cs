using System.Threading.Tasks;
using Database.Interfaces;
using Models;
using MongoDB.Driver;

namespace Database
{
    public class BankInfoRepository : IBankInfoRepository
    {        
        private const string CollectionName = "BankInfos";
        private readonly IMongoDatabase _database;

        private IMongoCollection<BankInfo> BankCollection => _database.GetCollection<BankInfo>(CollectionName);

        public BankInfoRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<BankInfo> GetAsync(string bik)
        {
            var result = await BankCollection.Find(x => x.Bik==bik)
                .SingleAsync();
            return result;
        }

        public async Task<BankInfo[]> FindAsync(string bik)
        {            
            var result = await BankCollection.Find(x=>x.Bik.Contains(bik)).ToListAsync();
            return result.ToArray();
        }

        public async Task AddManyAsync(BankInfo[] bankInfo)
        {
            foreach (var info in bankInfo)
            {                
                await BankCollection.ReplaceOneAsync(x=>x.Bik==info.Bik, info, new UpdateOptions{ IsUpsert = true });
            }
        }
    }
}
