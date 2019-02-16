using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            return (await this.Replace(videoList, v => v.Thumbnail.Url, v => v.Id)).ToList();
        }

        private async Task<IEnumerable<T>> Replace<T>(IEnumerable<T> data, Expression<Func<T, string>> memberLambda, Expression<Func<T, string>> idLambda)
        {
            var tasks = data.Select(async target =>
            {
                var memberSelectorExpression = memberLambda.Body as MemberExpression;
                var idSelectorExpression = idLambda.Body as MemberExpression;
                if (memberSelectorExpression != null && idSelectorExpression != null)
                {
                    var property = memberSelectorExpression.Member as PropertyInfo;
                    var idProperty = idSelectorExpression.Member as PropertyInfo;
                    if (property != null)
                    {
                        property.SetValue(target, await this.downloader.DownloadFile(
                            property.GetValue(target) as string, idProperty.GetValue(target) as string,
                            this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath), null);
                    }
                }
                return target;
            });

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result);
        }
    }
}
