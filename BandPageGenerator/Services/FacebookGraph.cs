using BandPageGenerator.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookGraph
    {
        private readonly Facebook config;
        private readonly ILogger logger;
        private readonly HttpClient client;

        public FacebookGraph(IOptions<Facebook> config, ILoggerFactory loggerFactory)
        {
            this.config = config.Value;
            this.logger = loggerFactory.CreateLogger<FacebookGraph>();
            this.client = new HttpClient
            {
                BaseAddress = new Uri($"https://graph.facebook.com/{this.config.ApiVersion}/")
            };
        }

        public async Task<int> GetPageLikeCountAsync()
        {
            JToken data = await this.GetGraphDataAsync(
                this.config.PageId, new[] { "fan_count" });

            return data["fan_count"].Value<int>();
        }

        private async Task<JToken> GetGraphDataAsync(string edge, string[] fields, Tuple<string, string>[] filters = null)
        {
            var queryString = $"{edge}?fields={string.Join(",", fields)}&access_token={this.config.AccessToken}";

            if (filters != null)
            {
                var filterString = string.Join("", filters.Select(i => $"&{i.Item1}={i.Item2}"));

                queryString += filterString;
            }

            var response = await this.client.GetStringAsync(queryString);

            return JToken.Parse(response);
        }
    }
}
