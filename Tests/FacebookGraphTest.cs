using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Tests
{
    public class FacebookGraphTest
    {
        [Fact]
        public async void ShouldNotThrow()
        {
            var client = new Mock<IFormattedHttpClient>();
            client.Setup(c => c.GetAsync<FacebookFanCountModel>(It.IsAny<string>()))
                .ReturnsAsync(new FacebookFanCountModel { FanCount = 3 });

            var options = new Mock<IOptions<Facebook>>();
            options.Setup(o => o.Value).Returns(new Facebook());

            var graph = new FacebookGraph(options.Object, client.Object);

            var value = await graph.GetPageLikeCountAsync();

            Assert.Equal(3, value);
        }
    }
}
