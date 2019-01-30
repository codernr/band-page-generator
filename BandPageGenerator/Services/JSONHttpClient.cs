using BandPageGenerator.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class JsonHttpClient<TNamingStrategy> : IJsonHttpClient<TNamingStrategy> where TNamingStrategy : NamingStrategy, new()
    {
        private readonly HttpClient client;
        private readonly JsonSerializer serializer;

        public JsonHttpClient(HttpClient client)
        {
            this.client = client;
            this.serializer = new JsonSerializer
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new TNamingStrategy() }
            };
        }

        public async Task<TModel> GetAsync<TModel>(string requestUri)
        {
            return await this.DeserializeAsync<TModel>(
                await this.client.GetAsync(requestUri));
        }

        public async Task<TResponse> PostAsync<TResponse>(string requestUri, HttpContent content)
        {
            return await this.DeserializeAsync<TResponse>(
                await this.client.PostAsync(requestUri, content));
        }

        private async Task<TModel> DeserializeAsync<TModel>(HttpResponseMessage responseTask)
        {
            using (Stream s = await responseTask.Content.ReadAsStreamAsync())
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return this.serializer.Deserialize<TModel>(reader);
            }
        }
    }
}
