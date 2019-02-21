using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class DownloaderClient : IDownloaderClient
    {
        private static readonly Dictionary<string, string> acceptedTypes = new Dictionary<string, string>
        {
            { "image/jpeg", "jpg" }
        };

        private readonly HttpClient client;
        private readonly ILogger<DownloaderClient> logger;
        private readonly IFileSystem fileSystem;

        public DownloaderClient(HttpClient client, ILogger<DownloaderClient> logger, IFileSystem fileSystem)
        {
            this.client = client;
            this.logger = logger;
            this.fileSystem = fileSystem;
        }

        public Task<string> DownloadFile(string requestUri, string id, string savePath, string basePath)
        {
            return this.DownloadFile(requestUri, id, savePath, basePath, false);
        }

        public async Task<string> DownloadFile(string requestUri, string id, string savePath, string basePath, bool forceDownload)
        {
            using (this.logger.BeginScope("Download file with id {0}", id))
            {
                var contentType = await this.GetContentTypeAsync(requestUri);

                if (!acceptedTypes.ContainsKey(contentType))
                {
                    this.logger.LogCritical("Not supported content type: {0}", contentType);
                    throw new NotSupportedException($"{contentType} is not supported for download");
                }

                var fileName = $"{id}.{acceptedTypes[contentType]}";

                this.fileSystem.Directory.CreateDirectory(savePath);

                var filePath = this.fileSystem.Path.Combine(savePath, fileName);
                var returnPath = this.fileSystem.Path.Combine(basePath, fileName);

                if (this.fileSystem.File.Exists(filePath) && !forceDownload)
                {
                    this.logger.LogWarning("The file {0} already exists, it is not saved again.", filePath);
                    return returnPath;
                }

                this.logger.LogInformation("Downloading from {0}", requestUri);

                var request = await this.client.GetAsync(requestUri);

                using (var contentStream = await request.Content.ReadAsStreamAsync())
                using (var fileStream = this.fileSystem.File.Create(filePath))
                {
                    await contentStream.CopyToAsync(fileStream);
                }

                return returnPath;
            }
        }

        private async Task<string> GetContentTypeAsync(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, requestUri);

            var response = await this.client.SendAsync(request);

            return response.Content.Headers.ContentType.ToString();
        }
    }
}
