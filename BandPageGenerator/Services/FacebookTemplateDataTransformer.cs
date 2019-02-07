using BandPageGenerator.Config;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly FacebookClient client;
        private readonly Facebook config;

        public FacebookTemplateDataTransformer(FacebookClient client, IOptions<Facebook> config)
        {
            this.client = client;
            this.config = config.Value;
        }

        public async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            // TODO: conditionally add instagram data

            templateData.Add("Likes", await this.client.GetPageLikeCountAsync());

            var events = await this.client.GetPageEventsAsync();

            templateData.Add("UpcomingEvents", events.Where(e => e.StartTime > DateTime.Now).ToArray());
            templateData.Add("PastEvents", events.Where(e => e.StartTime < DateTime.Now).ToArray().Take(this.config.PastEventDisplayLimit));

            templateData.Add("FeaturedPhotos", await this.client.GetFeaturedPhotosAsync());
            templateData.Add("InstagramPhotos", await this.client.GetRecentInstagramPhotosAsync());
        }
    }
}
