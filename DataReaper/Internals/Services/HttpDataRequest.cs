using System.Net.Http;
using System.Threading.Tasks;
using DataReaper.Internals.Services.Interfaces;

namespace DataReaper.Internals.Services
{
    public class HttpDataRequest : IHttpDataRequest
    {
        public Task<string> GetStringAsync(string requestUri)
        {
            var client = new HttpClient();
            return client.GetStringAsync(requestUri);
        }
    }
}
