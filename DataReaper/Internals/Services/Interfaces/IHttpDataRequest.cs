using System.Threading.Tasks;

namespace DataReaper.Internals.Services.Interfaces
{
    public interface IHttpDataRequest
    {
        Task<string> GetStringAsync(string requestUri);
    }
}
