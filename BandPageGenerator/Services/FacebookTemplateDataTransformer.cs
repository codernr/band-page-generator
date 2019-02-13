using BandPageGenerator.Config;
using BandPageGenerator.Models;
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

            templateData.Add("ProfilePictureUrl", await this.client.GetProfilePictureAsync());

            var events = await this.client.GetPageEventsAsync();

            templateData.Add("UpcomingEvents", events.Where(e => e.StartTime > DateTime.Now).ToArray());
            templateData.Add("PastEvents", events.Where(e => e.StartTime < DateTime.Now).ToArray().Take(this.config.PastEventDisplayLimit));

            templateData.Add("FeaturedPhotos", await this.GetFeaturedPhotosAsync());
            templateData.Add("MemberPhotos", await this.GetMemberPhotosAsync());
            templateData.Add("InstagramPhotos", await this.client.GetRecentInstagramPhotosAsync());
        }

        private async Task<List<FacebookPhotoModel>> GetFeaturedPhotosAsync()
        {
            var photos = await this.client.GetAlbumAsync(this.config.AlbumId);

            return photos.Select(p => p.Images.Aggregate((i1, i2) => i1.Height > i2.Height ? i1 : i2)).ToList();
        }

        private async Task<List<FacebookMemberPhotoModel>> GetMemberPhotosAsync()
        {
            var photos = await this.client.GetAlbumAsync(this.config.MembersAlbumId);

            return photos.Select(p =>
            {
                var data = p.Name.Split('\n');
                if (data.Length < 2) throw new FormatException("Member photos description has to have a new line in it!");

                var image = p.Images.Aggregate((i1, i2) => i1.Height > i2.Height ? i1 : i2);
                return new FacebookMemberPhotoModel
                {
                    Name = data[0],
                    Description = data[1],
                    Width = image.Width,
                    Height = image.Height,
                    Source = image.Source
                };
            }).ToList();
        }
    }
}
