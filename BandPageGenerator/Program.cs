using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BandPageGenerator
{
    class Program
    {
        [Option]
        public string SettingsPath { get; }

        [Option]
        public string TemplateDirectoryPath { get; }

        [Option]
        [Required]
        public string OutputPath { get; }

        [Option]
        public string DownloadSavePath { get; set; }

        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            var settings = this.SettingsPath ?? "appsettings.json";

            var serviceProvider = ServiceConfiguration.ConfigureServiceProvider(settings, this.DownloadSavePath);

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            string templatesDirectory = "Templates";

            if (this.TemplateDirectoryPath != null
                && Directory.Exists(this.TemplateDirectoryPath)
                && File.Exists(Path.Combine(this.TemplateDirectoryPath, "index.html")))
            {
                logger.LogInformation("Loading templates from external directory: " + templatesDirectory);
                templatesDirectory = this.TemplateDirectoryPath;
            }

            logger.LogInformation("Rendering to file: " + this.OutputPath);

            if (serviceProvider.GetRequiredService<IDownloaderClient>() is NullDownloaderClient)
            {
                logger.LogInformation("Image files are referenced directly from social pages");
            }
            else
            {
                logger.LogInformation("Image files are downloaded locally and served from site");
            }

            RenderToFileAsync(
                serviceProvider,
                Path.Combine(Directory.GetCurrentDirectory(), templatesDirectory, "index.html"),
                this.OutputPath).Wait();
        }

        static async Task RenderToFileAsync(IServiceProvider serviceProvider, string templatePath, string outputPath)
        {
            var renderer = serviceProvider.GetService<IViewRenderer>();

            var templateData = new Dictionary<string, object>();

            foreach (var clientService in serviceProvider.GetServices<ITemplateDataTransformer>())
            {
                await clientService.AddTemplateDataAsync(templateData);
            }

            await File.WriteAllTextAsync(outputPath, await renderer.RenderViewToStringAsync(templatePath, templateData), new UTF8Encoding(false));
        }
    }
}
