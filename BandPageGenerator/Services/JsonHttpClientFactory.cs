using System;
using System.Net.Http;
using System.Text.Json;
using BandPageGenerator.Services.Interfaces;

namespace BandPageGenerator.Services
{
    public class JsonHttpClientFactory : IJsonHttpClientFactory
    {
        private readonly HttpClient httpClient;

        public JsonHttpClientFactory(HttpClient httpClient) => this.httpClient = httpClient;
        
        public JsonHttpClient CreateClient(ClientNamingPolicy namingPolicy)
        {
            JsonNamingPolicy policy = namingPolicy switch
            {
                ClientNamingPolicy.CamelCase => JsonNamingPolicy.CamelCase,
                _ => throw new ArgumentException("Invalid enum type")
            };

            return new JsonHttpClient(this.httpClient, policy);
        }
    }
}