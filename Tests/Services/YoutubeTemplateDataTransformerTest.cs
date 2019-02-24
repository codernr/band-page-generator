using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public static class YoutubeTemplateDataTransformerTest
    {
        [Fact]
        public async static Task ShouldAddTemplateDataKeys()
        {
            var clientMock = new Mock<IYoutubeClient>();
            clientMock.Setup(c => c.GetCumulatedViewCount())
                .ReturnsAsync(12);
            clientMock.Setup(c => c.GetFeaturedVideos())
                .ReturnsAsync(new[] { new YoutubeVideoModel
                {
                    Id = "asdf",
                    PublishedAt = new DateTime(2019, 02, 21),
                    Title = "VideoTitle",
                    Description = "VideoDescription",
                    Thumbnail = new YoutubeThumbnailModel { Url = "http://example.com", Width = 100, Height = 50 }
                } });

            var downloaderMock = new Mock<IDownloaderClient>();
            downloaderMock.Setup(d => d.DownloadFile("http://example.com", "asdf", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("woaaaa");

            var optionsMock = new Mock<IOptions<GeneralConfig>>();
            optionsMock.Setup(o => o.Value).Returns(new GeneralConfig());

            var client = new YoutubeTemplateDataTransformer(clientMock.Object, downloaderMock.Object, optionsMock.Object);

            var templateData = new Dictionary<string, object>();

            await client.AddTemplateDataAsync(templateData);

            var video = (templateData["Videos"] as List<YoutubeVideoModel>)[0];

            Assert.Equal((long)12, templateData["ViewCount"]);
            Assert.Equal("asdf", video.Id);
            Assert.Equal(new DateTime(2019, 02, 21), video.PublishedAt);
            Assert.Equal("VideoTitle", video.Title);
            Assert.Equal("VideoDescription", video.Description);
            Assert.Equal(100, video.Thumbnail.Width);
            Assert.Equal(50, video.Thumbnail.Height);
            Assert.Equal("woaaaa", video.Thumbnail.Url);
        }
    }
}
