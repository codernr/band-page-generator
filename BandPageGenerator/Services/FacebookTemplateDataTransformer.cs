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
        private readonly FacebookConfig config;
        private readonly GeneralConfig generalConfig;
        private readonly DownloaderClient downloader;

        public FacebookTemplateDataTransformer(
            FacebookClient client, IOptions<FacebookConfig> config, DownloaderClient downloader,
            IOptions<GeneralConfig> generalConfig)
        {
            this.client = client;
            this.config = config.Value;
            this.generalConfig = generalConfig.Value;
            this.downloader = downloader;
        }

        public async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            // TODO: conditionally add instagram data

            templateData.Add("Likes", await this.client.GetPageLikeCountAsync());

            var profilePictureUri = await this.client.GetProfilePictureAsync();

            templateData.Add("ProfilePictureUrl", await this.downloader.DownloadFile(
                profilePictureUri, "profile", this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath));

            var events = await this.client.GetPageEventsAsync();

            templateData.Add("UpcomingEvents", events.Where(e => e.StartTime > DateTime.Now).ToArray());
            templateData.Add("PastEvents", events.Where(e => e.StartTime < DateTime.Now).ToArray().Take(this.config.PastEventDisplayLimit));

            templateData.Add("FeaturedPhotos", await this.GetFeaturedPhotosAsync());
            templateData.Add("MemberPhotos", await this.GetMemberPhotosAsync());
            templateData.Add("InstagramPhotos", await this.GetInstagramPhotos());
        }

        private async Task<List<FacebookPhotoModel>> GetFeaturedPhotosAsync()
        {
            var photos = await this.client.GetAlbumAsync(this.config.AlbumId);

            var download = this.Flatten(photos).Select(async item =>
            {
                item.Source = await this.downloader.DownloadFile(
                    item.Source, item.Id, this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath);
                return item;
            }).ToList();

            await Task.WhenAll(download);

            return download.Select(t => t.Result).ToList();
        }

        private async Task<List<FacebookMemberPhotoModel>> GetMemberPhotosAsync()
        {
            var photos = await this.client.GetAlbumAsync(this.config.MembersAlbumId);

            var tasks = photos.Select(async p =>
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
                    Source = await this.downloader.DownloadFile(
                        image.Source, p.Id, this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath)
                };
            }).ToList();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();
        }

        private async Task<List<FacebookInstagramMediaModel>> GetInstagramPhotos()
        {
            var photos = await this.client.GetRecentInstagramPhotosAsync();

            var tasks = photos.Select(async p =>
            {
                p.MediaUrl = await this.downloader.DownloadFile(
                    p.MediaUrl, p.Id, this.generalConfig.DownloadSavePath, this.generalConfig.DownloadedBasePath);
                return p;
            }).ToList();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();
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
