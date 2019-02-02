using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BandPageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceConfiguration.ConfigureServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            if (args.Length == 0)
            {
                logger.LogError("No output file path provided, exiting");
                Environment.Exit(-1);
            }

            var outputPath = args[0];

            logger.LogInformation("Rendering to file: " + outputPath);

            RenderToFileAsync(
                serviceProvider,
                Path.Combine(Directory.GetCurrentDirectory(), "Templates/index.html"),
                args[0]).Wait();
        }

        static async Task RenderToFileAsync(IServiceProvider serviceProvider, string templatePath, string outputPath)
        {
            var renderer = serviceProvider.GetService<IViewRenderer>();

            var templateData = new Dictionary<string, object>();

            foreach (var clientService in serviceProvider.GetServices<ITemplateDataTransformer>())
                await clientService.AddTemplateDataAsync(templateData);

            await File.WriteAllTextAsync(outputPath, await renderer.RenderViewToStringAsync(templatePath, templateData), Encoding.UTF8);
        }
    }
}
