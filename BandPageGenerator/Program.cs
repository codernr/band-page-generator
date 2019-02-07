﻿using BandPageGenerator.Services.Interfaces;
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
            var settings = "appsettings.json";

            if (args.Length == 3)
            {
                if (File.Exists(args[2]))
                {
                    settings = args[2];
                }
            }

            var serviceProvider = ServiceConfiguration.ConfigureServiceProvider(settings);

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            if (args.Length == 0)
            {
                logger.LogError("No output file path provided, exiting");
                Environment.Exit(-1);
            }

            string templatesDirectory = "Templates";

            if (args.Length > 1)
            {
                templatesDirectory = args[1];
                if (Directory.Exists(templatesDirectory) && File.Exists(Path.Combine(templatesDirectory, "index.html")))
                {
                    logger.LogInformation("Loading templates from external directory: " + templatesDirectory);
                }
            }

            var outputPath = args[0];

            logger.LogInformation("Rendering to file: " + outputPath);

            RenderToFileAsync(
                serviceProvider,
                Path.Combine(Directory.GetCurrentDirectory(), templatesDirectory, "index.html"),
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
