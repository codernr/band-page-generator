using BandPageGenerator.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<TModel> GetAsync<TModel>(string requestUri, (string, string)[] requestHeaders)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            foreach (var header in requestHeaders) request.Headers.Add(header.Item1, header.Item2);

            return await this.DeserializeAsync<TModel>(
                await this.client.SendAsync(request));
        }


        public async Task<TModel> PostAsync<TModel>(string requestUri, HttpContent content, (string, string)[] requestHeaders)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            foreach (var header in requestHeaders) request.Headers.Add(header.Item1, header.Item2);

            request.Content = content;

            return await this.DeserializeAsync<TModel>(
                await this.client.SendAsync(request));
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
