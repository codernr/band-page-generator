using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IDownloaderClient
    {
        Task<string> DownloadFile(string requestUri, string id, string savePath, string basePath, bool forceDownload = false);
    }
}
