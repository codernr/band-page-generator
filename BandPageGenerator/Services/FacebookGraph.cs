using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
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
        private readonly IFormattedHttpClient client;

        public FacebookGraph(IOptions<Facebook> config, ILoggerFactory loggerFactory, IFormattedHttpClient client)
        {
            this.config = config.Value;
            this.logger = loggerFactory.CreateLogger<FacebookGraph>();
            this.client = client;
        }

        public async Task<int> GetPageLikeCountAsync()
        {
            var data = await this.GetGraphDataAsync<FacebookFanCountModel>(
                this.config.PageId, new[] { "fan_count" });

            return data.FanCount;
        }

        private Task<TModel> GetGraphDataAsync<TModel>(string edge, string[] fields, Tuple<string, string>[] filters = null)
        {
            var queryString = string.Format("https://graph.facebook.com/{0}/{1}?fields={2}&access_token={3}",
                this.config.ApiVersion, edge, string.Join(",", fields), this.config.AccessToken);

            if (filters != null)
            {
                var filterString = string.Join("", filters.Select(i => $"&{i.Item1}={i.Item2}"));

                queryString += filterString;
            }

            return this.client.GetAsync<TModel>(queryString);
        }
    }
}
