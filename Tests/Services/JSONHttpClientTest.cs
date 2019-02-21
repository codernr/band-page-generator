using BandPageGenerator.Services;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class JsonHttpClientTest
    {
        [Fact]
        public async static Task GetShouldReturnModel()
        {
            var client = new JsonHttpClient<CamelCaseNamingStrategy>(GetClient("{\"id\": 1, \"name\": \"hello\"}"));

            var response = await client.GetAsync<TestModel>("http://example.com/any/url");
            
            Assert.Equal(1, response.Id);
            Assert.Equal("hello", response.Name);
        }

        [Fact]
        public async static Task GetWithHeadersShouldReturnModel()
        {
            var client = new JsonHttpClient<CamelCaseNamingStrategy>(GetClient("{\"id\": 1, \"name\": \"hello\"}"));

            var response = await client.GetAsync<TestModel>("http://example.com/any/url", new[] { ("Authorization", "Bearer asdf") });
            
            Assert.Equal(1, response.Id);
            Assert.Equal("hello", response.Name);
        }

        [Fact]
        public async static Task PostShouldReturnModel()
        {
            var client = new JsonHttpClient<CamelCaseNamingStrategy>(GetClient("{\"id\": 1, \"name\": \"hello\"}"));

            var response = await client.PostAsync<TestModel>("http://example.com/any/url", new StringContent("any content"), new[] { ("Authorization", "Bearer asdf") });

            Assert.Equal(1, response.Id);
            Assert.Equal("hello", response.Name);
        }

        [Fact]
        public async static Task ShouldThrowJsonException()
        {
            var client = new JsonHttpClient<CamelCaseNamingStrategy>(GetClient("sdsdsdsd"));

            await Assert.ThrowsAsync<JsonReaderException>(() => client.GetAsync<TestModel>("http://example.com"));
        }

        private static HttpClient GetClient(string responseContent)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            return new HttpClient(handlerMock.Object);
        }

        public class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
