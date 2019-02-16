using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class YoutubeTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly YoutubeClient client;
        private readonly DownloaderClient downloader;
        private readonly GeneralConfig generalConfig;

        public YoutubeTemplateDataTransformer(YoutubeClient client, DownloaderClient downloader, IOptions<GeneralConfig> generalConfig)
        {
            this.client = client;
            this.downloader = downloader;
            this.generalConfig = generalConfig.Value;
        }

        public async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            templateData.Add("ViewCount", await this.client.GetCumulatedViewCount());
            templateData.Add("Videos", await this.GetVideos());
        }

        private async Task<List<YoutubeVideoModel>> GetVideos()
        {
            var videoList = await this.client.GetFeaturedVideos();

            var tasks = videoList.Select(async v =>
            {
                v.Thumbnail.Url = await this.downloader.DownloadFile(
                    v.Thumbnail.Url, v.Id, this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath);
                return v;
            }).ToList();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();
        }
    }
}
