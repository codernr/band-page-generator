using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using System;
using System.IO;

namespace BandPageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddSingleton<IAsyncStubbleRenderer>(new StubbleBuilder().Build())
                .AddSingleton<IViewRenderer, StubbleViewRenderer>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            var renderer = serviceProvider.GetService<IViewRenderer>();

            var model = new { Name = "Anybody" };

            var renderTask = renderer.RenderViewToStringAsync<object>(
                Path.Combine(Directory.GetCurrentDirectory(), "Templates/index.html"),
                model);

            renderTask.Wait();

            logger.LogInformation(renderTask.Result);

            Console.ReadKey();
        }
    }
}
