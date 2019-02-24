using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public static class SpotifyClientTest
    {
        private static ILogger<SpotifyClient> LoggerMock => Mock.Of<ILogger<SpotifyClient>>();

        [Fact]
        public static async Task ShouldReturnCorrectAlbums()
        {
            var client = new Mock<IJsonHttpClient<SnakeCaseNamingStrategy>>();

            client.Setup(c => c.GetAsync<SpotifyPagingModel<SpotifySimplifiedAlbumModel>>(
                It.IsAny<string>(), It.IsAny<(string, string)[]>()))
                .ReturnsAsync(new SpotifyPagingModel<SpotifySimplifiedAlbumModel>
                {
                    Items = new[]
                {
                    new SpotifySimplifiedAlbumModel { Href = "aaa" },
                    new SpotifySimplifiedAlbumModel { Href = "bbb" }
                }
                });

            client.Setup(c => c.GetAsync<SpotifyAlbumModel>("aaa", It.IsAny<(string, string)[]>()))
                .ReturnsAsync(new SpotifyAlbumModel { Name = "AlbumA" });
            client.Setup(c => c.GetAsync<SpotifyAlbumModel>("bbb", It.IsAny<(string, string)[]>()))
                .ReturnsAsync(new SpotifyAlbumModel { Name = "AlbumB" });

            client.Setup(c => c.PostAsync<SpotifyClientCredentialsModel>(
                It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<(string, string)[]>()))
                .ReturnsAsync(new SpotifyClientCredentialsModel());

            var options = new Mock<IOptions<SpotifyConfig>>();
            options.Setup(o => o.Value).Returns(new SpotifyConfig());

            var spotifyClient = new SpotifyClient(options.Object, client.Object, LoggerMock);

            var results = await spotifyClient.GetAlbumsAsync();

            Assert.Single(results.Where(r => r.Name == "AlbumA"));
            Assert.Single(results.Where(r => r.Name == "AlbumB"));
        }
    }
}
