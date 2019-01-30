using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IFormattedHttpClient
    {
        Task<TModel> GetAsync<TModel>(string requestUri);

        Task<TModel> GetAsync<TModel>(string requestUri, (string, string)[] requestHeaders);

        Task<TResponse> PostAsync<TResponse>(string requestUri, HttpContent content, (string, string)[] requestHeaders);
    }
}
