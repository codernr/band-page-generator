using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public static class SpotifyTemplateDataTransformerTest
    {
        [Fact]
        public async static Task ShouldAddTemplateDataKeys()
        {
            var clientMock = new Mock<ISpotifyClient>();
            clientMock.Setup(c => c.GetAlbumsAsync())
                .ReturnsAsync(new[]
                {
                    new SpotifyAlbumModel
                    {
                        AlbumType = "album",
                        Id = "123",
                        Label = "Label",
                        Name = "AlbumName",
                        Type = "Single",
                        ReleaseDate = "2018",
                        ReleaseDatePrecision = "year",
                        Images = new[]
                        {
                            new SpotifyImageModel { Url = "http://example.com", Height = 10, Width = 20 }
                        },
                        Tracks = new SpotifyPagingModel<SpotifyTrackModel>
                        {
                            Items = new[]
                            {
                                new SpotifyTrackModel { DurationMs = 20000, Id = "track0", Name = "Track", TrackNumber = 1 }
                            }
                        }
                    }
                });

            var downloaderMock = new Mock<IDownloaderClient>();
            downloaderMock.Setup(d => d.DownloadFile("http://example.com", "123", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("woaaaa");

            var optionsMock = new Mock<IOptions<GeneralConfig>>();
            optionsMock.Setup(o => o.Value).Returns(new GeneralConfig());

            var spotifyOptionsMock = new Mock<IOptions<SpotifyConfig>>();
            spotifyOptionsMock.Setup(o => o.Value).Returns(new SpotifyConfig
            {
                AlternativeLinks = new[]
                {
                    new AlternativeLink { Title = "AlbumName", Links = new Dictionary<string, string>
                    {
                        { "bandcamp", "http://asdf.bandcamp.com" }
                    }
                    }
                }
            });

            var transformer = new SpotifyTemplateDataTransformer(clientMock.Object, spotifyOptionsMock.Object, downloaderMock.Object, optionsMock.Object);

            var data = new Dictionary<string, object>();

            await transformer.AddTemplateDataAsync(data);

            Assert.True(data.ContainsKey("Albums"));

            var albums = (data["Albums"] as IEnumerable<SpotifyAlbumTemplateModel>).ToArray();
            var album = albums[0];

            Assert.Equal("album", album.AlbumType);
            Assert.Equal("123", album.Id);
            Assert.Equal("Label", album.Label);
            Assert.Equal("Single", album.Type);
            Assert.Equal("AlbumName", album.Name);
            Assert.Equal(new DateTime(2018, 1, 1), album.ReleaseDate);
            Assert.Equal("woaaaa", album.Image.Url);
            Assert.Equal(10, album.Image.Height);
            Assert.Equal(20, album.Image.Width);
            Assert.Equal(20000, album.Tracks[0].DurationMs);
            Assert.Equal(TimeSpan.FromSeconds(20), album.Tracks[0].Duration);
            Assert.Equal("track0", album.Tracks[0].Id);
            Assert.Equal("Track", album.Tracks[0].Name);
            Assert.Equal(1, album.Tracks[0].TrackNumber);
            Assert.Equal("http://asdf.bandcamp.com", albums[0].AlternativeLinks["bandcamp"]);
        }
    }
}
