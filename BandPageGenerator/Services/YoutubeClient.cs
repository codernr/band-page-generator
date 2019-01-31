using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class YoutubeClient
    {
        private readonly Youtube config;
        private readonly IFormattedHttpClient client;
        private const string apiUri = "https://www.googleapis.com/youtube/v3/";

        public YoutubeClient(IOptions<Youtube> config, IJsonHttpClient<CamelCaseNamingStrategy> client)
        {
            this.config = config.Value;
            this.client = client;
        }

        public async Task<long> GetCumulatedViewCount()
        {
            var channelViewData = await this.GetApiDataAsync<YoutubeDataWrapperModel<YoutubeDataModel>>(
                "channels", ("part", "statistics"), ("id", this.config.ChannelId));

            long viewCount = channelViewData.Items[0].Statistics.ViewCount;

            if (this.config.AdditionalVideoIds != null)
            {
                var additionalViewData = await this.GetApiDataAsync<YoutubeDataWrapperModel<YoutubeDataModel>>(
                    "videos", ("part", "statistics"), ("id", string.Join(',', this.config.AdditionalVideoIds)));

                viewCount += additionalViewData.Items.Sum(i => i.Statistics.ViewCount);
            }

            return viewCount;
        }

        public async Task<YoutubeVideoModel[]> GetFeaturedVideos()
        {
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

            return this.client.GetAsync<TModel>(queryString);
        }
    }
}
