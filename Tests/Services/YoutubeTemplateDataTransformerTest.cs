using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class YoutubeTemplateDataTransformerTest
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
                    Thumbnail = new YoutubeThumbnailModel { Url = "http://example.com" }
                } });

            var downloaderMock = new Mock<IDownloaderClient>();
            downloaderMock.Setup(d => d.DownloadFile("http://example.com", "asdf", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("woaaaa");

            var optionsMock = new Mock<IOptions<GeneralConfig>>();
            optionsMock.Setup(o => o.Value).Returns(new GeneralConfig());

            var client = new YoutubeTemplateDataTransformer(clientMock.Object, downloaderMock.Object, optionsMock.Object);

            var templateData = new Dictionary<string, object>();

            await client.AddTemplateDataAsync(templateData);

            Assert.Equal((long)12, templateData["ViewCount"]);
            Assert.Equal("woaaaa", (templateData["Videos"] as List<YoutubeVideoModel>)[0].Thumbnail.Url);
        }
    }
}
