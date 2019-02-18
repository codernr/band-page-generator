using BandPageGenerator.Config;
using BandPageGenerator.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class FacebookTemplateDataTransformer : AbstractTemplateDataTransformer
    {
        private readonly FacebookClient client;
        private readonly FacebookConfig config;

        public FacebookTemplateDataTransformer(
            FacebookClient client, IOptions<FacebookConfig> config, DownloaderClient downloader,
            IOptions<GeneralConfig> generalConfig) : base(downloader, generalConfig)
        {
            this.client = client;
            this.config = config.Value;
        }

        public override async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            // TODO: conditionally add instagram data

            templateData.Add("Likes", await this.client.GetPageLikeCountAsync());

            var profilePictureUri = await this.client.GetProfilePictureAsync();

            templateData.Add("ProfilePictureUrl", await this.downloader.DownloadFile(
                profilePictureUri, "profile", this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath, true));

            var events = await this.client.GetPageEventsAsync();
            var upcomingEvents = events.Where(e => e.StartTime > DateTime.Now);
            var pastEvents = events.Where(e => e.StartTime < DateTime.Now).Take(this.config.PastEventDisplayLimit);

            templateData.Add("UpcomingEvents", await this.Replace(upcomingEvents, e => e.Cover.Source, e => e.Cover.Id));
            templateData.Add("PastEvents", await this.Replace(pastEvents, e => e.Cover.Source, e => e.Cover.Id));

            var featuredPhotos = this.Flatten(await this.client.GetAlbumAsync(this.config.AlbumId));
            templateData.Add("FeaturedPhotos", await this.Replace(featuredPhotos, p => p.Source, p => p.Id));

            var memberPhotos = await this.GetTransformedMemberPhotosAsync();
            templateData.Add("MemberPhotos", await this.Replace(memberPhotos, p => p.Source, p => p.Id));

            var instagramPhotos = await this.client.GetRecentInstagramPhotosAsync();
            templateData.Add("InstagramPhotos", await this.Replace(instagramPhotos, p => p.MediaUrl, p => p.Id));
        }

        private async Task<IEnumerable<FacebookMemberPhotoModel>> GetTransformedMemberPhotosAsync()
        {
            var photos = await this.client.GetAlbumAsync(this.config.MembersAlbumId);

            return photos.Select(p =>
            {
                var data = p.Name.Split('\n');
                if (data.Length < 2) throw new FormatException("Member photos description has to have a new line in it!");

                var image = p.Images.Aggregate((i1, i2) => i1.Height > i2.Height ? i1 : i2);
                return new FacebookMemberPhotoModel
                {
                    Id = p.Id,
                    Name = data[0],
                    Description = data[1],
                    Width = image.Width,
                    Height = image.Height,
                    Source = image.Source
                };
            });
        }

        private IEnumerable<FacebookPhotoModel> Flatten(List<FacebookAlbumPhotosModel> album)
        {
            return album.Select(item =>
            {
                var transformed = item.Images.Aggregate((i1, i2) => i1.Height > i2.Height ? i1 : i2);
                transformed.Id = item.Id;
                return transformed;
            });
        }
    }
}
