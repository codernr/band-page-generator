using BandPageGenerator.Services.Interfaces;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class NullDownloaderClient : IDownloaderClient
    {
        public Task<string> DownloadFile(string requestUri, string id, string savePath, string basePath, bool forceDownload = false)
        {
            return Task.FromResult(requestUri);
        }
    }
}
