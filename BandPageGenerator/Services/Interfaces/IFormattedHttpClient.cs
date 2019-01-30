using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IFormattedHttpClient
    {
        Task<TModel> GetAsync<TModel>(string requestUri);

        Task<TResponse> PostAsync<TResponse>(string requestUri, HttpContent content);
    }
}
