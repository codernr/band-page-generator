using System;
using System.Net.Http;
using System.Text.Json;

namespace BandPageGenerator.Services
{
    public class JsonHttpClientFactory
    {
        public enum NamingPolicy
        {
            CamelCase
        }

        private readonly HttpClient httpClient;
        public JsonHttpClientFactory(HttpClient httpClient) => this.httpClient = httpClient;
        
        public JsonHttpClient CreateClient(NamingPolicy namingPolicy)
        {
            JsonNamingPolicy policy = namingPolicy switch
            {
                NamingPolicy.CamelCase => JsonNamingPolicy.CamelCase,
                _ => throw new ArgumentException("Invalid enum type")
            };

            return new JsonHttpClient(this.httpClient, policy);
        }
    }
}