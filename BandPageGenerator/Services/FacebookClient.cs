using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookClient : IFacebookClient
    {
        private readonly FacebookConfig config;
        private readonly IFormattedHttpClient client;
        private readonly ILogger<FacebookClient> logger;

        public FacebookClient(IOptions<FacebookConfig> config, IJsonHttpClient<SnakeCaseNamingStrategy> client, ILogger<FacebookClient> logger)
        {
            this.config = config.Value;
            this.client = client;
            this.logger = logger;
        }

        public async Task<int> GetPageLikeCountAsync()
        {
            this.logger.LogInformation("Retrieving facebook fan count...");

            var data = await this.GetGraphDataAsync<FacebookFanCountModel>(
                this.config.PageId, new[] { "fan_count" });

            this.logger.LogInformation("Fan count: {0}", data.FanCount);

            return data.FanCount;
        }

        public Task<List<FacebookEventModel>> GetPageEventsAsync()
        {
            this.logger.LogInformation("Retrieving page events...");

            return this.GetPagedGraphDataAsync<FacebookEventModel>(
                $"{this.config.PageId}/events",
                new[] { "cover", "category", "description", "end_time", "name", "place", "start_time", "ticket_uri" },
                new[] { ("event_state_filter", "[\"published\"]") });
        }

        /// <summary>
        /// Gets the photos from the featured album with the biggest resolution
        /// </summary>
        public async Task<List<FacebookAlbumPhotosModel>> GetAlbumAsync(string albumId)
        {
            this.logger.LogInformation("Retrieving album photos (album id: {0}) ...", albumId);

            return await this.GetPagedGraphDataAsync<FacebookAlbumPhotosModel>(
                $"{albumId}/photos", new[] { "images", "link", "name" });
        }

        public async Task<FacebookInstagramMediaModel[]> GetRecentInstagramPhotosAsync()
        {
            this.logger.LogInformation("Retrieving Instagram recent photos...");

            var media = await this.GetGraphDataAsync<FacebookListModel<FacebookInstagramMediaModel>>(
                $"{this.config.InstagramId}/media", new[] { "media_type", "media_url", "caption", "permalink", "timestamp" });

            var images = media.Data.Where(m => m.MediaType == "IMAGE");

            if (this.config.FilterHashtags != null && this.config.FilterHashtags.Length > 0)
            {
                images = images.Where(i => i.Caption != null && this.config.FilterHashtags.Any(filter => i.Caption.Contains(filter)));
            }

            return images.ToArray();
        }

        public async Task<string> GetProfilePictureAsync()
        {
            this.logger.LogInformation("Retrieving profile picture...");

            var data = await this.GetGraphDataAsync<FacebookDataModel<FacebookProfilePictureModel>>(
                $"{this.config.PageId}/picture", new[] { "url" }, new[] { ("redirect", "0"), ("width", "1200") });

            return data.Data.Url;
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

            this.logger.LogInformation("Querying Graph API endpoint: {0}", queryString);

            return this.client.GetAsync<TModel>(queryString);
        }
    }
}
