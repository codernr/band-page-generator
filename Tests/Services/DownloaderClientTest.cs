using BandPageGenerator.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public static class DownloaderClientTest
    {
        private static ILogger<DownloaderClient> LoggerMock => Mock.Of<ILogger<DownloaderClient>>();

        [Fact]
        public async static Task ShouldReturnString()
        {
            var httpClient = GetClient("image/jpeg");

            var fileSystem = new MockFileSystem();

            var downloader = new DownloaderClient(httpClient, LoggerMock, fileSystem);

            var response = await downloader.DownloadFile("http://example.com", "ABC", "C:\\aaa", "http://mypage.com/download");

            Assert.Equal(Path.Combine("http://mypage.com/download", "ABC.jpg"), response);
        }

        [Fact]
        public async static Task ShouldThrowNotSupportedException()
        {
            var httpClient = GetClient("application/json");

            var fileSystem = new MockFileSystem();

            var downloader = new DownloaderClient(httpClient, LoggerMock, fileSystem);

            await Assert.ThrowsAsync<NotSupportedException>(() => downloader.DownloadFile("http://example.com", "ABC", "C:\\aaa", "http://mypage.com/download"));
        }

        private static HttpClient GetClient(string contentType)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new MockHttpContent("asdf", contentType)
                });

            return new HttpClient(handlerMock.Object);
        }

        private class MockHttpContent : StringContent
        {
            public MockHttpContent(string content, string contentType) : base(content)
            {
                this.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
        }
    }
}
