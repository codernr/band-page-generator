using BandPageGenerator.Services.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class JsonHttpClient : IFormattedHttpClient
    {
        private readonly HttpClient client;
        private readonly JsonSerializer serializer;

        public JsonHttpClient(HttpClient client, JsonSerializer serializer)
        {
            this.client = client;
            this.serializer = serializer;
        }

        public async Task<TModel> GetAsync<TModel>(string requestUri)
        {
            using (Stream s = await this.client.GetStreamAsync(requestUri))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return this.serializer.Deserialize<TModel>(reader);
            }
        }
    }
}
