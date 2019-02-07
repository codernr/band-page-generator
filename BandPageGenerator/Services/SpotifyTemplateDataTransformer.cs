using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class SpotifyTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly SpotifyClient client;

        public SpotifyTemplateDataTransformer(SpotifyClient client) => this.client = client;

        public async Task AddTemplateDataAsync(Dictionary<string, object> templateData)
        {
            var albums = await this.client.GetAlbumsAsync();
            templateData.Add("Albums", albums.Select(a => this.Map(a)).ToArray());
        }

        private SpotifyAlbumTemplateModel Map(SpotifyAlbumModel model)
        {
            return new SpotifyAlbumTemplateModel
            {
                AlbumType = model.AlbumType,
                Id = model.Id,
                Label = model.Label,
                Name = model.Name,
                ReleaseDate = this.MapReleaseDate(model.ReleaseDate, model.ReleaseDatePrecision),
                Type = model.Type,
                Image = model.Images.Aggregate((i1, i2) => i1.Height > i2.Height ? i1 : i2),
                Tracks = model.Tracks.Items.OrderBy(t => t.TrackNumber).ToArray()
            };
        }

        private DateTime? MapReleaseDate(string releaseDate, string releaseDatePrecision)
        {
            if (string.IsNullOrEmpty(releaseDate)) return null;

            var parts = releaseDate.Split('-');

            switch (releaseDatePrecision)
            {
                case "year":
                    return new DateTime(int.Parse(releaseDate), 0, 0);

                case "month":
                    return new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), 0);

                case "day":
                    return new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));

                default:
                    return null;
            }
        }
    }
}
