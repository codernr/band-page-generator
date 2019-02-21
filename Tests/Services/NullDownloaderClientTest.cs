using BandPageGenerator.Services;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public class NullDownloaderClientTest
    {
        [Fact]
        public static async Task ShouldReturnRequestUri()
        {
            var client = new NullDownloaderClient();

            var response0 = await client.DownloadFile("asdf", "x", "z", "t");
            var response1 = await client.DownloadFile("qwer", "x", "z", "t", true);

            Assert.Equal("asdf", response0);
            Assert.Equal("qwer", response1);
        }
    }
}
