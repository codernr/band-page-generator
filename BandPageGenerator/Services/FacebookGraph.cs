using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookGraph
    {
        private readonly Facebook config;
        private readonly IFormattedHttpClient client;

        public FacebookGraph(IOptions<Facebook> config, IJsonHttpClient<SnakeCaseNamingStrategy> client)
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

        public Task<List<FacebookEventModel>> GetPageEventsAsync()
        {
            return this.GetPagedGraphDataAsync<FacebookEventModel>(
                $"{this.config.PageId}/events",
                new[] { "cover", "category", "description", "end_time", "name", "start_time", "ticket_uri" },
                new[] { ("event_state_filter", "[\"published\"]") });
        }

        /// <summary>
        /// Gets the photos from the featured album with the biggest resolution
        /// </summary>
        public async Task<List<FacebookPhotoModel>> GetFeaturedPhotosAsync()
        {
            var photos = await this.GetPagedGraphDataAsync<FacebookAlbumPhotosModel>(
                $"{this.config.AlbumId}/photos", new[] { "images", "link" });

            return photos.Select(p => p.Images.Aggregate((i1, i2) => i1.Height > i2.Height ? i1 : i2)).ToList();
        }

        private async Task<List<TModel>> GetPagedGraphDataAsync<TModel>(string edge, string[] fields, (string, string)[] filters = null)
        {
            var dataList = new List<TModel>();

            var data = await this.GetGraphDataAsync<FacebookListModel<TModel>>(edge, fields, filters);

            dataList.AddRange(data.Data);

            while (!string.IsNullOrEmpty(data.Paging?.Next))
            {
                data = await this.client.GetAsync<FacebookListModel<TModel>>(data.Paging.Next);

                dataList.AddRange(data.Data);
            }

            return dataList;
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
