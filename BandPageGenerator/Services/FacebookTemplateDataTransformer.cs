using BandPageGenerator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly FacebookClient client;

        public FacebookTemplateDataTransformer(FacebookClient client) => this.client = client;

        public async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            // TODO: conditionally add instagram data

            templateData.Add("Likes", await this.client.GetPageLikeCountAsync());

            var events = await this.client.GetPageEventsAsync();

            templateData.Add("UpcomingEvents", events.Where(e => e.StartTime.DateTime > DateTime.Now).ToArray());
            templateData.Add("PastEvents", events.Where(e => e.StartTime.DateTime < DateTime.Now).ToArray());

            templateData.Add("FeaturedPhotos", await this.client.GetFeaturedPhotosAsync());
            templateData.Add("InstagramPhotos", await this.client.GetRecentInstagramPhotosAsync());
            System.Console.WriteLine("addtemplate");
        }
    }
}
