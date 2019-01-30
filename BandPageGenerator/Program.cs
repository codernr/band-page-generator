using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace BandPageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceConfiguration.ConfigureServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            var renderer = serviceProvider.GetService<IViewRenderer>();

            var model = new { Name = "Anybody" };

            var renderTask = renderer.RenderViewToStringAsync<object>(
                Path.Combine(Directory.GetCurrentDirectory(), "Templates/index.html"),
                model);

            renderTask.Wait();

            logger.LogInformation(renderTask.Result);

            var fbTask = serviceProvider.GetService<FacebookClient>().GetPageLikeCountAsync();
            fbTask.Wait();
            logger.LogInformation("Count: " + fbTask.Result);

            Console.ReadKey();
        }
    }
}
