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

        public async Task<int> GetPageLikeCount()
        {
            JToken data = await this.GetGraphData(this.config.PageId, "fan_count");

            return data["fan_count"].Value<int>();
        }

        private async Task<JToken> GetGraphData(string edge, params string[] fields)
        {
            var response = await this.client.GetStringAsync(
                $"{edge}?fields={string.Join(",", fields)}&access_token={this.config.AccessToken}");

            return JToken.Parse(response);
        }
    }
}
