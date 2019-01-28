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
            var client = this.CreateClient(new FacebookFanCountModel { FanCount = 3 });

            var options = this.CreateOptions(new Facebook());

            var graph = new FacebookGraph(options.Object, client.Object);

            var value = await graph.GetPageLikeCountAsync();

            Assert.Equal(3, value);
        }

        [Fact]
        public async void ShouldReturnArray()
        {
            var client = this.CreateClient<FacebookListModel<FacebookEventModel>>(new FacebookListModel<FacebookEventModel>
            {
                Data = new[] { new FacebookEventModel(), new FacebookEventModel() }
            });

            var options = this.CreateOptions(new Facebook());

            var graph = new FacebookGraph(options.Object, client.Object);

            var response = await graph.GetPageEventsAsync();

            Assert.Equal(2, response.Length);
        }

        private Mock<IFormattedHttpClient> CreateClient<TModel>(TModel returnValue)
        {
            var client = new Mock<IFormattedHttpClient>();
            client.Setup(c => c.GetAsync<TModel>(It.IsAny<string>()))
                .ReturnsAsync(returnValue);
            return client;
        }

        private Mock<IOptions<Facebook>> CreateOptions(Facebook returnValue)
        {
            var options = new Mock<IOptions<Facebook>>();
            options.Setup(o => o.Value).Returns(returnValue);
            return options;
        }
    }
}
