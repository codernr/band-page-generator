using BandPageGenerator.Services.Interfaces;
using System.Text.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class JsonHttpClient : IFormattedHttpClient
    {
        private readonly HttpClient client;
        private readonly JsonSerializerOptions serializerOptions;

        public JsonHttpClient(HttpClient client, JsonNamingPolicy policy)
        {
            this.client = client;
            this.serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = policy
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
            {
                return await JsonSerializer.DeserializeAsync<TModel>(s, this.serializerOptions);
            }
        }
    }
}
