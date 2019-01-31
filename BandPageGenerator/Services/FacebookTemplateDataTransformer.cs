using BandPageGenerator.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly FacebookClient client;

        public FacebookTemplateDataTransformer(FacebookClient client) => this.client = client;

        public async Task AddTemplateData(Dictionary<string, object> templateData)
        {
            // TODO: conditionally add instagram data

            templateData.Add("Facebook", new
            {
                Likes = await this.client.GetPageLikeCountAsync(),
                Events = await this.client.GetPageEventsAsync(),
                Photos = await this.client.GetFeaturedPhotosAsync()
            });
        }
    }
}
