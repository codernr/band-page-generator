using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class DownloaderClient
    {
        private static readonly Dictionary<string, string> acceptedTypes = new Dictionary<string, string>()
        {
            { "image/jpeg", "jpg" }
        };

        private readonly HttpClient client;

        public DownloaderClient(HttpClient client) => this.client = client;

        public async Task<string> DownloadFile(string requestUri, string id, string savePath, string basePath, bool forceDownload = false)
        {
            var request = await this.client.GetAsync(requestUri);

            var contentType = request.Content.Headers.ContentType.ToString();

            if (!acceptedTypes.ContainsKey(contentType)) throw new NotSupportedException($"{contentType} is not supported for download");

            var fileName = $"{id}.{acceptedTypes[contentType]}";

            Directory.CreateDirectory(savePath);

            var filePath = Path.Combine(savePath, fileName);
            var returnPath = Path.Combine(basePath, fileName);

            if (File.Exists(filePath) && !forceDownload) return returnPath;

            using (var contentStream = await request.Content.ReadAsStreamAsync())
            using (var fileStream = File.Create(filePath))
            {
                await contentStream.CopyToAsync(fileStream);
            }

            return returnPath;
        }
    }
}
