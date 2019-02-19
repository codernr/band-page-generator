using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Tests
{
    public class FacebookClientTest
    {
        private ILogger<FacebookClient> LoggerMock => Mock.Of<ILogger<FacebookClient>>();

        [Fact]
        public async void ShouldNotThrow()
        {
            var client = this.CreateClient(new FacebookFanCountModel { FanCount = 3 });

            var options = this.CreateOptions(new FacebookConfig());

            var graph = new FacebookClient(options.Object, client.Object, this.LoggerMock);

            var value = await graph.GetPageLikeCountAsync();

            Assert.Equal(3, value);
        }

        [Fact]
        public async void ShouldReturnArray()
        {
            var client = this.CreateClient(new FacebookListModel<FacebookEventModel>
            {
                Data = new[] { new FacebookEventModel(), new FacebookEventModel() }
            });

            var options = this.CreateOptions(new FacebookConfig());

            var graph = new FacebookClient(options.Object, client.Object, this.LoggerMock);

            var response = await graph.GetPageEventsAsync();

            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async void ShouldReturnAlbumList()
        {
            var client = this.CreateClient(new FacebookListModel<FacebookAlbumPhotosModel>
            {
                Data = new[]
                {
                    new FacebookAlbumPhotosModel
                    {
                        Images = new[] { new FacebookPhotoModel { Height = 1 }, new FacebookPhotoModel { Height = 2 } }
                    },
                    new FacebookAlbumPhotosModel
                    {
                        Images = new[] { new FacebookPhotoModel { Height = 3 }, new FacebookPhotoModel { Height = 4 } }
                    }
                }
            });

            var options = this.CreateOptions(new FacebookConfig());

            var graph = new FacebookClient(options.Object, client.Object, this.LoggerMock);

            var photos = await graph.GetAlbumAsync("aaa");

            Assert.Equal(2, photos.Count);
            Assert.Equal(2, photos[0].Images[1].Height);
            Assert.Equal(4, photos[1].Images[1].Height);
        }

        [Fact]
        public async void ShouldReturnAllPagesOfData()
        {
            var client = new Mock<IJsonHttpClient<SnakeCaseNamingStrategy>>();
            client.Setup(c => c.GetAsync<FacebookListModel<FacebookEventModel>>(It.IsNotIn("nextPage")))
                .ReturnsAsync(new FacebookListModel<FacebookEventModel>
                {
                    Data = new[] { new FacebookEventModel(), new FacebookEventModel() },
                    Paging = new FacebookPagingModel { Next = "nextPage" }
                });
            client.Setup(c => c.GetAsync<FacebookListModel<FacebookEventModel>>("nextPage"))
                .ReturnsAsync(new FacebookListModel<FacebookEventModel>
                {
                    Data = new[] { new FacebookEventModel() }
                });

            var options = this.CreateOptions(new FacebookConfig());

            var graph = new FacebookClient(options.Object, client.Object, this.LoggerMock);

            var response = await graph.GetPageEventsAsync();

            Assert.Equal(3, response.Count);
        }

        [Fact]
        public async void ShouldFilterInstagramImages()
        {
            var client = this.CreateClient(new FacebookListModel<FacebookInstagramMediaModel> { Data = new[] {
                new FacebookInstagramMediaModel { MediaType = "IMAGE" },
                new FacebookInstagramMediaModel { MediaType = "VIDEO" }
            }});

            var options = this.CreateOptions(new FacebookConfig());

            var facebookClient = new FacebookClient(options.Object, client.Object, this.LoggerMock);

            var response = await facebookClient.GetRecentInstagramPhotosAsync();

            Assert.Single(response);
        }

        [Fact]
        public async void ShouldFilterInstagramImageHashtags()
        {
            var client = this.CreateClient(new FacebookListModel<FacebookInstagramMediaModel>
            {
                Data = new[] {
                new FacebookInstagramMediaModel { MediaType = "IMAGE", Caption = "no hashtag" },
                new FacebookInstagramMediaModel { MediaType = "VIDEO", Caption = "#hashtag1 but in video" },
                new FacebookInstagramMediaModel { MediaType = "IMAGE", Caption = "#hashtag2 in image" },
                new FacebookInstagramMediaModel { MediaType = "IMAGE", Caption = "#notdefined hashtag in image" }
            }
            });

            var options = this.CreateOptions(new FacebookConfig { FilterHashtags = new[] { "#hashtag1", "#hashtag2" } });

            var facebookClient = new FacebookClient(options.Object, client.Object, this.LoggerMock);

            var response = await facebookClient.GetRecentInstagramPhotosAsync();

            Assert.Single(response);
            Assert.Contains("#hashtag2", response[0].Caption);
        }

        private Mock<IJsonHttpClient<SnakeCaseNamingStrategy>> CreateClient<TModel>(TModel returnValue)
        {
            var client = new Mock<IJsonHttpClient<SnakeCaseNamingStrategy>>();
            client.Setup(c => c.GetAsync<TModel>(It.IsAny<string>()))
                .ReturnsAsync(returnValue);
            return client;
        }

        private Mock<IOptions<FacebookConfig>> CreateOptions(FacebookConfig returnValue)
        {
            var options = new Mock<IOptions<FacebookConfig>>();
            options.Setup(o => o.Value).Returns(returnValue);
            return options;
        }
    }
}
