using System.Threading.Tasks;
using BankDictionary.Services;
using Database.Interfaces;
using Models;
using Moq;
using Xunit;

namespace XUnitTest
{
    public class BankInfoSearchServiceTest
    {
        private BankInfoSearchService service;
        Mock<IBankInfoRepository> _repository;

        public BankInfoSearchServiceTest()
        {
            _repository = new Mock<IBankInfoRepository>(MockBehavior.Strict);
            service = new BankInfoSearchService(_repository.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task FindEmpty(string bik)
        {
            var result = await service.SearchAsync(bik);
            Assert.Empty(result);
            Assert.Collection(result);
        }

        [Fact]
        public async Task FindPart()
        {
            var bik = "001";
            _repository.Setup(repository => repository.FindAsync("001")).Returns(Task.FromResult(new BankInfo[0]));
            await service.SearchAsync(bik);
            _repository.Verify(repository=> repository.FindAsync("001"));
        }

        [Fact]
        public async Task FindAll ()
        {
            var bik = "044257464";
            _repository.Setup(repository => repository.GetAsync("044257464")).Returns(Task.FromResult(new BankInfo()));
            await service.SearchAsync(bik);
            _repository.Verify(repository => repository.GetAsync("044257464"));
        }
    }
}
