using BandPageGenerator.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class YoutubeTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly YoutubeClient client;

        public YoutubeTemplateDataTransformer(YoutubeClient client) => this.client = client;

        public async Task AddTemplateData(Dictionary<string, object> templateData)
        {
            templateData.Add("Youtube", new
            {
                ViewCount = await this.client.GetCumulatedViewCount(),
                FeaturedVideos = await this.client.GetFeaturedVideos();
            });
        }
    }
}
