using BandPageGenerator.Config;
using BandPageGenerator.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class YoutubeTemplateDataTransformer : AbstractTemplateDataTransformer
    {
        private readonly YoutubeClient client;

        public YoutubeTemplateDataTransformer(YoutubeClient client, DownloaderClient downloader, IOptions<GeneralConfig> generalConfig)
            : base(downloader, generalConfig) => this.client = client;

        public override async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            templateData.Add("ViewCount", await this.client.GetCumulatedViewCount());
            templateData.Add("Videos", await this.GetVideos());
        }

        private async Task<List<YoutubeVideoModel>> GetVideos()
        {
            var videoList = await this.client.GetFeaturedVideos();

            return (await this.Replace(videoList, v => v.Thumbnail.Url, v => v.Id)).ToList();
        }
    }
}
