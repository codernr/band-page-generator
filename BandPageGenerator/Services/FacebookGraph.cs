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
        private readonly IFormattedHttpClient client;

        public FacebookGraph(IOptions<Facebook> config, IFormattedHttpClient client)
        {
            this.config = config.Value;
            this.client = client;
        }

        public async Task<int> GetPageLikeCountAsync()
        {
            var data = await this.GetGraphDataAsync<FacebookFanCountModel>(
                this.config.PageId, new[] { "fan_count" });

            return data.FanCount;
        }

        // TODO: returns only first page of data
        public async Task<FacebookEventModel[]> GetPageEventsAsync()
        {
            var data = await this.GetGraphDataAsync<FacebookListModel<FacebookEventModel>>(
                $"{this.config.PageId}/events",
                new[] { "cover", "category", "description", "end_time", "name", "start_time", "ticket_uri" },
                new[] { ("event_state_filter", "[\"published\"]") });

            return data.Data;
        }

        private Task<TModel> GetGraphDataAsync<TModel>(string edge, string[] fields, (string, string)[] filters = null)
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
