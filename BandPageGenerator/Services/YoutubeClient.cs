using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class YoutubeClient
    {
        private readonly YoutubeConfig config;
        private readonly IFormattedHttpClient client;
        private readonly ILogger<YoutubeClient> logger;
        private const string apiUri = "https://www.googleapis.com/youtube/v3/";

        public YoutubeClient(
            IOptions<YoutubeConfig> config,
            IJsonHttpClient<CamelCaseNamingStrategy> client,
            ILogger<YoutubeClient> logger)
        {
            this.config = config.Value;
            this.client = client;
            this.logger = logger;
        }

        public async Task<long> GetCumulatedViewCount()
        {
            this.logger.LogInformation("Retrieving view count for channel (id: {0})...", this.config.ChannelId);

            var channelViewData = await this.GetApiDataAsync<YoutubeDataWrapperModel<YoutubeDataModel>>(
                "channels", ("part", "statistics"), ("id", this.config.ChannelId));

            long viewCount = channelViewData.Items[0].Statistics.ViewCount;

            if (this.config.AdditionalVideoIds != null)
            {
                var videoIds = string.Join(',', this.config.AdditionalVideoIds);

                this.logger.LogInformation("Retrieving view count for videos (id: {0})...", videoIds);

                var additionalViewData = await this.GetApiDataAsync<YoutubeDataWrapperModel<YoutubeDataModel>>(
                    "videos", ("part", "statistics"), ("id", videoIds));

                viewCount += additionalViewData.Items.Sum(i => i.Statistics.ViewCount);
            }

            return viewCount;
        }

        public async Task<YoutubeVideoModel[]> GetFeaturedVideos()
        {
            this.logger.LogInformation("Retrieving featured videos from playlist (id: {0})...", this.config.FeaturedPlaylistId);

            var data = await this.GetApiDataAsync<YoutubeDataWrapperModel<YoutubeSnippetModel>>(
                "playlistItems", ("maxResults", "50"), ("playlistId", this.config.FeaturedPlaylistId), ("part", "snippet"));

            return data.Items.Select(i => new YoutubeVideoModel(i)).ToArray();
        }

        private Task<TModel> GetApiDataAsync<TModel>(string edge, params (string, string)[] parameters)
        {
            var queryString = string.Format("{0}{1}?key={2}{3}",
                apiUri,
                edge,
                this.config.ApiKey,
                parameters != null ? string.Join("", parameters.Select(i => $"&{i.Item1}={i.Item2}")) : string.Empty);

            this.logger.LogInformation("Querying Youtube Data API endpoint: {0}", queryString);

            return this.client.GetAsync<TModel>(queryString);
        }
    }
}
