using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IFormattedHttpClient
    {
        Task<TModel> GetAsync<TModel>(string requestUri);
    }
}
